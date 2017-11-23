require literal.fs
require llclause.fs
require llclauseset.fs

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

100 constant max-depth

: see-clause { clause working seen -- working' seen' }
	clause seen resolve-with-set
	seen swap substract-set
	working merge-sets clause swap remove-clause
	clause seen insert-clause
	;
	
\ 0 ... not satisfiable
\ -1 ... satisfiable
\ 1 ... exceeded maximum recursion depth

: is-sat' recursive { working seen depth -- flag }
\	cr cr ." iteration: " depth .
\	cr ." working: " working show-set
\	cr ." seen: " seen show-set
	working 0= IF
		-1
	ELSE
		0 working contains-clause IF
			0
		ELSE
			depth max-depth >= IF
				1
			ELSE
				working set-clause @ working seen see-clause
				depth 1+  is-sat'
			ENDIF
		ENDIF
	ENDIF
	;
	
: is-sat ( set -- flag ) 0 0 is-sat' ;
