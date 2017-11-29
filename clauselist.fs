require list.fs

list%
    cell% field clauselist-clause
end-struct clauselist%

: pop-clause ( list<clause*> -- list<clause*> clause* )
    list-pop swap clauselist-clause ;

: new-clause-list-node { clause next -- list }
    clauselist% %alloc
    dup clause swap clauselist-clause !
    dup 0 swap list-next ! ;

: append-if-new recursive { clause clauselist -- clauselist }
    clauselist 0= IF
        clause 0 new-clause-list-node
    ELSE
        clauselist clauselist-clause @ clause clauses-equal IF
            clauselist
        ELSE
            clause clauselist list-next @ append-if-new
            clauselist list-next !
            clauselist
        ENDIF
    ENDIF ;

: contains-clause ( clause* list<clause*> -- f )
    { clause list } ['] clauselist-clause ['] clauses-equal clause list list-search ;
