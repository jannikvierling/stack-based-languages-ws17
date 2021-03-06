require clause.fs

struct
    cell% field set-next
    cell% field set-clause
end-struct set%

: set-size ( set -- n ) clause.size ;

\ Deep copy
: copy-set ( set1 -- set2 ) recursive
    { set } set 0= IF
        0
    ELSE
        set set-next @ copy-set
        set% %allot dup rot rot set-next !
        set set-clause @ clause.copy over set-clause !
    ENDIF ;

: remove-clause ( clause set1 -- set2 ) recursive
    { clause set } set 0= IF
        0
    ELSE
        set set-clause @ clause clause.equal? IF
            set set-next @
        ELSE
            clause set set-next @ clause.remove_literal
            set set-next !
            set
        ENDIF
    ENDIF ;

\ Just insert at the end (if didn't find the clause traversing the set)
: insert-clause ( clause set1 -- set2 ) recursive
    { clause set } set 0= IF
        set% %allot
        0 over set-next !
        clause over set-clause !
    ELSE set set-clause @ clause clause.equal? IF
            set
		ELSE
			clause set set-next @ clause.insert_literal
			set set-next !
			set
		ENDIF ENDIF ;
		
: substract-set recursive { settosubstract set -- set' }
	settosubstract 0= IF
		set
	ELSE
		settosubstract set-next @
		settosubstract set-clause @ set remove-clause
		substract-set
	ENDIF
	;

: show-set' ( set1 -- ) recursive
    dup 0<> IF
        dup
        set-clause @ clause.show
        set-next @ show-set'
    ENDIF ;

: show-set ( set -- )
    ." { " show-set' ." }"
    drop ;

: merge-sets' ( set1 set2 -- clause ) recursive
    { set1 set2 } set2 0= IF
        set1
    ELSE
        set2 set-clause @ set1 insert-clause
        set2 set-next @
        merge-sets'
    ENDIF ;
        
: merge-sets ( set1 set2 -- set )
    { set1 set2 } set1 clause.copy set2 merge-sets' ;
	
: contains-clause ( clause set -- x ) recursive
    dup 0= IF 2drop false
    ELSE
        2dup set-clause @ clause.equal? IF
            2drop true
        ELSE
            set-next @ contains-clause
        ENDIF
    ENDIF ;

