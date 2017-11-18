struct
    cell% field clause-next
    cell% field clause-literal
end-struct clause%

: clause-size ( clause -- n ) recursive
    { clause }  clause 0= IF
        0
    ELSE
        clause clause-next @ clause-next 1+
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
