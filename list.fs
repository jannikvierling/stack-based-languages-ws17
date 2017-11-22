struct
    cell% field list-next
end-struct list%

list%
    cell% field clauselist-clause
end-struct clauselist%

variable working 0 working !
variable number-seen 0 number-seen !
create seen

: list-length ( list -- n ) recursive
    \ "list" is a pointer to the first element of a linked list
    \ "n" is the length of the list.
    { list } list 0= IF
        0
    ELSE
        list list-next @ list-length 1+
    ENDIF ;

: pop-clause ( clauselist -- clauselist clause )
    dup clauselist-clause @ swap list-next @ swap ;

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

: contains-clause recursive { clause clauselist -- x }
    clauselist 0= IF false
    ELSE
        clauselist clauselist-clause @ clause clauses-equal IF
            true
        ELSE
            clause clauselist list-next @ contains-clause
        ENDIF
    ENDIF ;
