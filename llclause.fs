struct
    cell% field clause-next
    cell% field clause-literal
end-struct clause%

: literal-equal { l1 l2 -- flag } l1 l2 = ;

: literal-negated { l1 l2 -- flag } l1 l2 negate = ;

: literal-less { l1 l2 -- flag }
	l1 abs l2 abs <
		l1 abs l2 abs =
		l1 l2 <
		and
	or
	;

: literal-greater { l1 l2 -- flag }
	l1 l2 literal-less l1 l2 literal-equal or invert
	;

: clause-size ( clause -- n ) recursive
    { clause }  clause 0= IF
        0
    ELSE
        clause clause-next @ clause-size 1+
    ENDIF ;

: copy-clause ( clause1 -- clause2 ) recursive
    { clause } clause 0= IF
        0
    ELSE
        clause clause-next @ copy-clause
        clause% %allot dup rot rot clause-next !
        clause clause-literal @ over clause-literal !
    ENDIF ;

: remove-literal ( literal clause1 -- clause2 ) recursive
    { literal clause } clause 0= IF
        0
    ELSE
        clause clause-literal @ literal = IF
            clause clause-next @
        ELSE
            literal clause clause-next @ remove-literal
            clause clause-next !
            clause
        ENDIF
    ENDIF ;

: insert-literal ( literal clause1 -- clause2 ) recursive
    { literal clause } clause 0= IF
        clause% %allot
        0 over clause-next !
        literal over clause-literal !
    ELSE clause clause-literal @ literal literal-greater IF
            clause% %allot dup dup
            clause swap clause-next !
            literal swap clause-literal !
        ELSE clause clause-literal @ literal literal-less IF
                literal clause clause-next @ insert-literal
                clause clause-next !
                clause
            ELSE
                clause
            ENDIF ENDIF ENDIF ;

: show-clause' ( clause1 -- ) recursive
    dup 0<> IF
        dup
        clause-literal @ dec.
        clause-next @ show-clause'
    ENDIF ;

: show-clause ( clause -- )
    ." [ " show-clause' ." ]"
    drop ;

: merge-clauses' ( clause1 clause2 -- clause ) recursive
    { clause1 clause2 } clause2 0= IF
        clause1
    ELSE
        clause2 clause-literal @ clause1 insert-literal
        clause2 clause-next @
        merge-clauses'
    ENDIF ;
        
: merge-clauses ( clause1 clause2 -- clause )
    { clause1 clause2 } clause1 copy-clause clause2 merge-clauses' ;
		
: fast-merge' recursive { result clause1 clause2 -- } 
	clause1 0= IF
		clause2 copy-clause result clause-next !
	ELSE
		clause2 0= IF
			clause1 copy-clause result clause-next !
		ELSE
			\ Almost identical parts of code repeated three times
			\ Could be a lot shorter with smarter usage of IF but
			\ also a lot less readable
			clause1 clause-literal @ clause2 clause-literal @ literal-equal IF
				clause% %allot
				0 over clause-next !	
				clause1 clause-literal @ over clause-literal !
				dup result clause-next !
				clause1 clause-next @ clause2 clause-next @ fast-merge'
			ELSE
				clause1 clause-literal @ clause2 clause-literal @ literal-less IF
					clause% %allot
					0 over clause-next !	
					clause1 clause-literal @ over clause-literal !
					dup result clause-next !
					clause1 clause-next @ clause2 fast-merge'
				ELSE
					clause% %allot
					0 over clause-next !	
					clause2 clause-literal @ over clause-literal !
					dup result clause-next !
					clause1 clause2 clause-next @ fast-merge'
				ENDIF
			ENDIF
		ENDIF
	ENDIF ;

\ Create a dummy first element to work with, then drop it
: fast-merge { clause1 clause2 -- clause }
	clause% %allot
	0 over clause-next !
	0 over clause-literal !
	dup	clause1 clause2 fast-merge'
	clause-next @ ;
	
\ Todo: Free the copies.
\ Question: Doesn't gforth have some sort of garbage collection?
: resolve-clauses ( literal clause1 clause2 -- resolvent )
    { literal clause1 clause2 }
    literal clause1 copy-clause remove-literal
    literal negate clause2 copy-clause remove-literal
    fast-merge ;

: clauses-equal ( clause1 clause2 -- x ) recursive
    \ Compares two clauses for equality.
    \ clause1, clause2: Adresses to clauses.
    \ x: -1 if the clauses are equal, 0 otherwise.
    { clause1 clause2 }
    clause1 0= IF
        clause2 0= IF true ELSE false ENDIF
    ELSE
        clause2 0= IF false
        ELSE
            clause1 clause-literal @ clause2 clause-literal @ =
            clause1 clause-next @ clause2 clause-next @
            clauses-equal and
        ENDIF
    ENDIF ;

: contains-literal ( literal clause -- x ) recursive
    \ Checks whether if a given clause contains a given literal.
    \
    \ x: A boolean value containing -1 if the literal is contained in the
    \ clause, 0 otherwise.
    dup 0= IF 2drop false
    ELSE
        2dup clause-literal @ = IF
            2drop true
        ELSE
            clause-next @ contains-literal
        ENDIF
    ENDIF ;

	
	
struct
    cell% field set-next
    cell% field set-clause
end-struct set%

: set-size ( set -- n ) clause-size ;

\ Deep copy
: copy-set ( set1 -- set2 ) recursive
    { set } set 0= IF
        0
    ELSE
        set set-next @ copy-set
        set% %allot dup rot rot set-next !
        set set-clause @ copy-clause over set-clause !
    ENDIF ;

: remove-clause ( clause set1 -- set2 ) recursive
    { clause set } set 0= IF
        0
    ELSE
        set set-clause @ clause clauses-equal IF
            set set-next @
        ELSE
            clause set set-next @ remove-literal
            set set-next !
            set
        ENDIF
    ENDIF ;

\ Just insert at the end (if didn't find the clause traversing the set)
: insert-clause ( clause set1 -- set2 ) recursive
    { clause set } set 0= IF
        set% %allot
        0 over set-next !
        clause over set-clause !
    ELSE set set-clause @ clause clauses-equal IF
            set
		ELSE
			clause set set-next @ insert-literal
			set set-next !
			set
		ENDIF ENDIF ;

: show-set' ( set1 -- ) recursive
    dup 0<> IF
        dup
        set-clause @ show-clause
        set-next @ show-set'
    ENDIF ;

: show-set ( set -- )
    ." { " show-set' ." }"
    drop ;

: merge-sets' ( set1 set2 -- clause ) recursive
    { set1 set2 } set2 0= IF
        set1
    ELSE
        set2 set-clause @ set1 insert-clause
        set2 set-next @
        merge-sets'
    ENDIF ;
        
: merge-sets ( set1 set2 -- set )
    { set1 set2 } set1 copy-clause set2 merge-sets' ;
	
: contains-clause ( clause set -- x ) recursive
    dup 0= IF 2drop false
    ELSE
        2dup set-clause @ clauses-equal IF
            2drop true
        ELSE
            set-next @ contains-clause
        ENDIF
    ENDIF ;
	

\ todo: Is is possible to simplify this word by computing the
\ resolvable atoms first?.
\ In particular it can be done in linear (instead of quadratic) time
\ Then again wlog all clauses have size 3 so it does not really matter
: resolve-all ( clause1 clause2 -- res_1 ... res_n n )
    { clause1 clause2 }
    0 clause1 BEGIN
        dup WHILE
            dup clause-literal @ negate clause2 contains-literal IF
                dup clause-literal @ clause1 clause2 resolve-clauses
                rot 1+ rot
            ENDIF
            clause-next @
    REPEAT drop ;

: times ( n "name" -- )
    ' { xt } 0 u+do xt execute loop ;

: make-set-from-stack' recursive ( clause_1 ... clause_n n result -- set )
	swap
	dup 0= IF
		drop
	ELSE
		1- rot rot insert-literal
		make-set-from-stack'
	ENDIF ;
	
: make-set-from-stack ( clause_1 ... clause_n n  -- set )
	0 make-set-from-stack' ; 
	
: merge-sets-from-stack' recursive ( clause_1 ... clause_n n result -- set )
	swap
	dup 0= IF
		drop
	ELSE
		1- rot rot merge-sets
		merge-sets-from-stack'
	ENDIF ;
	
: merge-sets-from-stack ( set_1 ... set_n n  -- set )
	0 merge-sets-from-stack' ; 
	
: resolve-with-set recursive { clause set -- set2 }
	set 0= IF
		0
	ELSE
		set BEGIN
			dup WHILE
				dup set-clause @ clause resolve-all
				make-set-from-stack
				swap set-next @
		REPEAT drop
		set set-size merge-sets-from-stack
	ENDIF ;

10 constant max-depth

: see-clause { clause working seen -- working' seen' }
	clause seen resolve-with-set
	working merge-sets clause swap remove-clause
	clause seen insert-clause
	;
	
\ 0 ... not satisfiable
\ -1 ... satisfiable
\ 1 ... exceeded maximum recursion depth

: is-sat' recursive { working seen depth }
	cr ." iteration: " depth .
	cr ." working: " working show-set
	cr ." seen: " seen show-set
	working 0= IF
		-1
	ELSE
		0 working contains-clause IF
			0
		ELSE
			depth max-depth > IF
				1
			ELSE
				working set-clause @ working seen see-clause
				depth 1+  is-sat'
			ENDIF
		ENDIF
	ENDIF
	;
	
: is-sat ( set -- depth ) 0 0 is-sat' ;

