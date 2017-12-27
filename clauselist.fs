
require list.fs
require clause.fs

list%
    cell% field clauselist-clause
end-struct clauselist%

: contains-clause ( clause* list<clause*> -- f )
    { clause list } ['] clauselist-clause ['] clauses-equal clause list list-search ;

: is_subsumed? ( clause* list<clauselist*> -- f )
    { clause list } ['] clauselist-clause [']  subsumed? clause list list-search ;

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

: last-node ( list -- last )
    BEGIN dup list-next @ 0<> WHILE
            list-next @
    REPEAT ;
    
: is_emptylist? ( list -- f )
    0= ;

: append ( clause clauselist -- clauselist )
    dup is_emptylist? GUARD
        drop 0 new-clause-list-node END
    tuck last-node swap 0 new-clause-list-node swap list-next ! ;

: append-new ( clause clauselist -- clauselist )
    2dup contains-clause GUARD
        nip END
    append ;

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