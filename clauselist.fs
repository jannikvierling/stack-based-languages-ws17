
require list.fs
require clause.fs

list%
    cell% field clauselist-clause
end-struct clauselist%

: contains-clause ( clause* list<clause*> -- f )
    { clause list } ['] clauselist-clause ['] clauses-equal clause list list-search ;

: show' ( clauselist -- )
    clauselist-clause @ show-clause ;

: show-clauselist ( clauselist -- )
    { list } [Char] } list 10 ['] show' [Char] { list-show ;

: pop-clause ( list<clause*> -- list<clause*> clause* )
    list-pop swap clauselist-clause @ ;

: new-clause-list-node { clause next -- list }
    clauselist% %alloc
    dup clause swap clauselist-clause !
    dup next swap list-next ! ;

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

: length-comparator ( clause1 clause2 -- f )
    list-length swap list-length swap < ;
    
: io-iterate? ( list clause < -- f )
    { list clause < } list 0= GUARD
        false END
    list clauselist-clause @ clause < execute ;

: insert-node ( prev current clause -- list )
    { list } swap new-clause-list-node { prev new }
    prev 0= GUARD
        new END
    new prev list-next ! list ;
    
: insert-ordered' ( list comparator clause -- list )
    { list < clause } 0 list BEGIN          ( loop invariant: prev current -- )
        dup clause < io-iterate? WHILE
            nip dup list-next @
    REPEAT clause list insert-node ;
                    
: insert-ordered ( comparator clause list -- list' )
    2dup contains-clause GUARD
        nip nip END
    rot rot insert-ordered' ;

: insert-by-length ( clause list -- list' )
    ['] length-comparator -rot insert-ordered ;

: insert-clause ( clause list -- list' )
    insert-by-length ;