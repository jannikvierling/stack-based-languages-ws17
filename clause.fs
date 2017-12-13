require literal.fs
require list.fs

list%
    cell% field clause-literal
end-struct clause%

: clause-next list-next ;

: clause-size list-length ;

: copy-clause ( clause1 -- clause2 ) recursive
    \ Copies a clause.
    { clause } clause 0= IF
        0
    ELSE
        clause clause-next @ copy-clause
        clause% %allot dup rot rot clause-next !
        clause clause-literal @ over clause-literal !
    ENDIF ;

: remove-literal ( literal clause1 -- clause2 ) recursive
    \ Removes a literal from a clause.
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
    \ Inserts a literal in a clause.
    \
    \ The "literal" is inserted into "clause1" only if this literal is
    \ not yet present. The resulting clause "clause2" is ordered.
    { literal clause } clause 0= IF
        clause% %allot
        0 over clause-next !
        literal over clause-literal !
    ELSE clause clause-literal @ literal > IF
            clause% %allot dup dup
            clause swap clause-next !
            literal swap clause-literal !
        ELSE clause clause-literal @ literal < IF
                literal clause clause-next @ insert-literal
                clause clause-next !
                clause
            ELSE
                clause
            ENDIF ENDIF ENDIF ;

: show-clause' ( clause -- ) clause-literal @ 1 .r ;

: show-clause ( clause -- )
    \ Prints a clause.
    { clause } [Char] ] clause bl ['] show-clause' [Char] [ list-show ;

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
			clause1 clause-literal @ clause2 clause-literal @ = IF
				clause% %allot
				0 over clause-next !	
				clause1 clause-literal @ over clause-literal !
				dup result clause-next !
				clause1 clause-next @ clause2 clause-next @ fast-merge'
			ELSE
				clause1 clause-literal @ clause2 clause-literal @ < IF
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

: clauses-equal ( clause1 clause2 -- f ) recursive
    \ Compares two clauses for equality.
    \
    \ "clause1", "clause2": Adresses to clauses.
    \ "f": true if the clauses are equal, false otherwise.
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

: contains-literal ( literal clause -- f)
    ['] clause-literal ['] = 2swap list-search ;

: resolve-clauses ( literal clause1 clause2 -- resolvent )
    \ Resolves two clauses upon a literal.
    \
    \ "literal": The resolved upon literal; this is a literal of
    \ clause1 and its dual must appear in clause2.
    { literal clause1 clause2 }
    literal clause1 copy-clause remove-literal
    literal negate clause2 copy-clause remove-literal
    fast-merge ; \ todo: free the copies

: resolve-all ( clause1 clause2 -- res_1 ... res_n n )
    \ Resolves two clauses upon all resolvable literals.
    { clause1 clause2 }
    0 clause1 BEGIN
        dup WHILE
            dup clause-literal @ negate clause2 contains-literal IF
                dup clause-literal @ clause1 clause2 resolve-clauses
                rot 1+ rot
            ENDIF
            clause-next @
    REPEAT drop ;
