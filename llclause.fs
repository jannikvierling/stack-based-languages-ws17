struct
    cell% field clause-next
    cell% field clause-literal
end-struct clause%

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

: show-clause' ( clause1 -- ) recursive
    dup 0<> IF
        dup
        clause-literal @ dec.
        clause-next @ show-clause'
    ENDIF ;

: show-clause ( clause -- )
    ." [ " show-clause' ." ]" ;

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
            
\ Todo: Free the copies.
: resolve-clauses ( literal clause1 clause2 -- resolvent )
    { literal clause1 clause2 }
    literal clause1 copy-clause remove-literal
    literal negate clause2 copy-clause remove-literal
    merge-clauses ;

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
            and clause1 clause-next @ clause2 clause-next @
            clauses-equal
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


\ todo: Is is possible to simplify this word by computing the
\ resolvable atoms first?.
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
    
