require literal.fs
require list.fs

list%
    cell% field clause-literal
end-struct clause%

: GUARD ( Compilation: -- orig; Run-time: f -- )
    POSTPONE IF ; immediate

: END ( Compilation: orig -- )
    POSTPONE EXIT POSTPONE ENDIF ; immediate

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
    { literal clause }
    clause 0= GUARD
        0 END
    clause clause-literal @ literal = GUARD
        clause clause-next @ END
    literal clause clause-next @ remove-literal
    clause clause-next !
    clause ;

: insert-literal ( literal clause1 -- clause2 ) recursive
    \ Inserts a literal in a clause.
    \
    \ The "literal" is inserted into "clause1" only if this literal is
    \ not yet present. The resulting clause "clause2" is ordered.
    { literal clause }
    clause 0= GUARD
        clause% %allot
        0 over clause-next !
        literal over clause-literal ! END
    clause clause-literal @ literal literal-greater GUARD
        clause% %allot dup dup
        clause swap clause-next !
        literal swap clause-literal ! END
    clause clause-literal @ literal literal-less GUARD
        literal clause clause-next @ insert-literal
        clause clause-next !
        clause END        
    clause ;

: show-clause' ( clause -- ) clause-literal @ 1 .r ;

: show-clause ( clause -- )
    \ Prints a clause.
    { clause } [Char] ] clause bl ['] show-clause' [Char] [ list-show ;

: new-clause-node ( literal next -- node )
    clause% %allot tuck clause-next ! tuck clause-literal ! ;

: fm-choose-args { clause1 clause2 -- c1' c2' l }
    clause1 clause-literal @ clause2 clause-literal @ literal-equal GUARD
         clause1 clause-next @ clause2 clause-next @ clause1 clause-literal @ END
    clause1 clause-literal @ clause2 clause-literal @ literal-less GUARD
         clause1 clause-next @ clause2 clause1 clause-literal @ END
    clause1 clause2 clause-next @ clause2 clause-literal @ ;

: fast-merge' recursive { result clause1 clause2 -- } 
	clause1 0= GUARD
		clause2 copy-clause result clause-next ! END
    clause2 0= GUARD
        clause1 copy-clause result clause-next ! END
    clause1 clause2 fm-choose-args
    0 new-clause-node dup result clause-next ! -rot
    fast-merge' ;

\ Create a dummy first element to work with, then drop it
: fast-merge ( clause1 clause2 -- clause )
    0 0 new-clause-node dup 2swap fast-merge'
	clause-next @ ;

: clauses-equal ( clause1 clause2 -- f ) recursive
    \ Compares two clauses for equality.
    \
    \ "clause1", "clause2": Adresses to clauses.
    \ "f": true if the clauses are equal, false otherwise.
    { clause1 clause2 }
    clause1 0= GUARD
        clause2 0= IF true ELSE false ENDIF END
    clause2 0= GUARD
        false END
    clause1 clause-literal @ clause2 clause-literal @ =
    clause1 clause-next @ clause2 clause-next @
    clauses-equal and ;

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
