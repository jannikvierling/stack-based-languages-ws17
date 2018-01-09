
require clause.fs
require clauselist.fs

: begin-dimacs ( -- 0 ) 0 ;

: parse-clause ( 0 l1 ... ln -- clause )
    0 BEGIN swap dup 0<> WHILE
            swap clause.insert_literal
    REPEAT drop ;

: end-dimacs ( dimacs clauses -- clauselist )
    0 swap 0 u+do >r parse-clause r> clauselist.insert_clause loop ;

: clauses ( .. 0 clauses -- ... clauses ) nip ;

