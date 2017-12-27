struct
    cell% field list-next
end-struct list%

: list.free_node ( list -- )
    free drop ;

: list.free ( list deallocator -- )
    { deallocator } BEGIN dup 0<> WHILE
            dup list-next @ swap dup deallocator execute list.free_node
    REPEAT drop ;

: last-node ( head -- last )
    BEGIN dup list-next @ 0<> WHILE
            list-next @
    REPEAT ;
    
: is_emptylist? ( head -- f )
    0= ;

: list-search { selector comparator val list -- f }
    list BEGIN dup 0<> WHILE dup selector execute @ val swap comparator execute IF
                drop true EXIT
            ENDIF list-next @ REPEAT drop false ;

: list-length ( list -- n)
    0 swap BEGIN
        dup 0<> WHILE list-next @ swap 1+ swap
    REPEAT drop ;

: list-pop ( list1 -- list1 list2 )
    dup list-next @ ;

: list-show ( end list delim show-node start -- )
    emit { delim show-node  -- } delim emit
    BEGIN dup 0<> WHILE
            dup show-node execute delim emit
            list-next @
    REPEAT drop emit ;
    
