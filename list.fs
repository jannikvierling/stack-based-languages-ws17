struct
    cell% field list.next
end-struct list%

: list.free_node ( list -- )
    \ Frees given list's head.
    free drop ;

: list.free ( list deallocator -- )
    \ Frees a list.
    \
    \ deallocator ( list -- )
    \
    \ Frees each node of the list using the deallocator to free the
    \ data references by the list.
    { deallocator } BEGIN dup 0<> WHILE
            dup list-next @ swap dup deallocator execute list.free_node
    REPEAT drop ;

: list.last ( head -- last )
    \ Retrieves the last element of a non-empty list.
    BEGIN dup list-next @ 0<> WHILE
            list-next @
    REPEAT ;
    
: list.is_empty? ( head -- f )
    \ Tests whether a list is empty.
    \
    \ If the given list is empty, then f is true, otherwise f is
    \ false.
    0= ;

: list.search ( selector comparator value list -- f )
    \ Searches a list for an element.
    \
    \ selector ( list -- T )
    \ comparator ( T T -- f ), where T is an arbitrary type.
    \
    \ Searches the given list for 'value' using 'selector' to select
    \ the data references by the list nodes and 'comparator' to
    \ compare the elements to 'value'. If 'value' matches one of the
    \ elements in list with respect to the comparator, then f is true,
    \ otherwise f is false.
    { selector comparator val list -- f }
    list BEGIN dup 0<> WHILE dup selector execute @ val swap comparator execute IF
                drop true EXIT
            ENDIF list-next @ REPEAT drop false ;

: list.length ( list -- n)
    \ Computes a list's length.
    0 swap BEGIN
        dup 0<> WHILE list-next @ swap 1+ swap
    REPEAT drop ;

: list.pop ( list1 -- list1 list2 )
    \ Pops the first element of a list.
    \
    \ This operation does not modify the list, indeed it just returns
    \ the original head and the next referenced element.
    dup list-next @ ;

: list.show ( end list delim show-node start -- )
    \ Prints a list to the screen.
    \
    \ "start" the list opening symbol.
    \ "end" the list closing symbol.
    \ "delim" the delimiter symbol.
    \ "show-node" ( node -- ).
    emit { delim show-node  -- } delim emit
    BEGIN dup 0<> WHILE
            dup show-node execute delim emit
            list-next @
    REPEAT drop emit ;
    
