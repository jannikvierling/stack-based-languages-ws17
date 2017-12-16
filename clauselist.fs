
require list.fs
require clause.fs

list%
    cell% field clauselist-clause
end-struct clauselist%

: pop-clause ( list<clause*> -- list<clause*> clause* )
    list-pop swap clauselist-clause @ ;

: new-clause-list-node { clause next -- list }
    clauselist% %alloc
    dup clause swap clauselist-clause !
    dup 0 swap list-next ! ;

: append-if-new ( clause clauselist -- clauselist )
    dup 0= GUARD
        drop 0 new-clause-list-node END
    tuck 0 BEGIN
            drop
            over over clauselist-clause @ clauses-equal IF
                2drop EXIT
            ENDIF dup list-next @ swap
        over 0=
        UNTIL nip swap 0 new-clause-list-node swap list-next ! ;
    
: contains-clause ( clause* list<clause*> -- f )
    { clause list } ['] clauselist-clause ['] clauses-equal clause list list-search ;

: show' ( clauselist -- ) clauselist-clause @ show-clause ;

: show-clauselist ( clauselist -- )
    { list } [Char] } list 10 ['] show' [Char] { list-show ;