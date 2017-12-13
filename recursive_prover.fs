require literal.fs
require clause.fs
require clauseset.fs

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

\ Set to negative value for unlimited depth
variable max-depth'
100 max-depth' !
: MAX-DEPTH max-depth' @ ;
: set-max-depth ( new-depth -- ) max-depth' ! ;

variable verbose'
FALSE verbose' !
: IS-VERBOSE verbose' @ ;
: verbose TRUE verbose' ! ;
: silent FALSE verbose' ! ;

: see-clause { clause working seen -- working' seen' }
    clause seen resolve-with-set
	seen swap substract-set
	working merge-sets clause swap remove-clause
    clause seen insert-clause ;
	
: is-sat' recursive { working seen depth -- working' seen' flag }

	IS-VERBOSE IF
	cr
	." -----------------------"			cr
	." Iteration: "  depth .			cr
	." Seen: "		seen	show-set	cr
	." Working: "	working	show-set	ENDIF

    working 0= IF
        working seen -1 EXIT ENDIF
	0 working contains-clause IF
        working seen 0 EXIT ENDIF
    depth MAX-DEPTH = IF
        working seen 1 EXIT ENDIF
    working set-clause @ working seen see-clause
    depth 1+  is-sat' ;
	
: is-sat ( set -- working seen flag )
    \ Refutes the given clause set.
    \ flag :0 if UNSAT, -1 if SAT, 1 recursion depth exceeded
    0 0 is-sat' ;

: refute ( set -- )
    is-sat

	IS-VERBOSE IF
	cr
	." ======================="			cr
    ." Seen:"    swap show-set 			cr
    ." Working:" swap show-set
	ELSE swap drop swap drop ENDIF
	
	cr
    dup  0 = IF ." UNSAT" ENDIF
    dup -1 = IF ." SAT"   ENDIF
    dup  1 = IF ." EXCEEDED RECURSION DEPTH" ENDIF
    drop ;

    
