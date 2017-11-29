struct
    cell% field list-next
end-struct list%

: list-search { selector comparator val list -- f }
    list BEGIN dup 0<> WHILE dup selector execute @ val swap comparator execute IF
                drop true exit
            ENDIF list-next @ REPEAT drop false ;

: list-length ( list -- n ) recursive
    \ "list" is a pointer to the first element of a linked list
    \ "n" is the length of the list.
    { list } list 0= IF
        0
    ELSE
        list list-next @ list-length 1+
    ENDIF ;

: list-pop ( list1 -- list1 list2 )
    dup list-next @ ;
