require control.fs
require literal.fs
require list.fs

list%
    cell% field clause.literal
end-struct clause%

: clause.deallocator ( node -- )
    \ Frees the memory held by a clause node.
    \
    \ "node" The node whose memory is to be freed.
    drop ;

: clause.free ( clause -- )
    \ Frees a clause node
    \
    \ "clause" The node to be freed.
    ['] clause.deallocator list.free ;

: clause.next ( clause -- clause + offset )
    \ Clause next-accessor.
    \
    \ "clause" The clause whose next component is to be accessed.
    \ Returns a the clause address to which the next-offset is added.
    list.next ;

: clause->literal ( clause -- literal )
    clause.literal @ ;

: clause.size ( clause -- n )
    \ Computes a given clause's size.
    \
    \ "clause" The clause whose size is to be computed.
    \ "n" The given clause's size.
    list.length ;

: clause.new_node ( literal next -- node )
    \ Creates a new clause node.
    \
    \ "literal" The literal to be associated to the new node.
    \ "next" The successor of the newly created node.
    \ Return the address of the newly allocated node.
    clause% %alloc tuck clause.next ! tuck clause.literal ! ;

: clause.copy ( clause1 -- clause2 ) recursive
    \ Copies a clause.
    \
    \ Creates a deep copy of the clause. Returns the address of the
    \ copy.
    { clause } clause list.is_empty? GUARD
        0 END
    clause clause.literal @
    clause clause.next @ clause.copy
    clause.new_node ;

: clause.remove_literal ( literal clause1 -- clause2 ) recursive
    \ Removes a literal from a clause.
    \
    \ "literal" The literal that is to be removed from the clause.
    \ "clause" The clause from which the literal is to be removed.
    \ The clause's address might change. Returns the address of the
    \ clause after removal of the literal.
    { literal clause }
    clause list.is_empty? GUARD
        0 END
    clause clause.literal @ literal = GUARD
         clause clause.next @ clause list.free_node END
    literal clause clause.next @ clause.remove_literal
    clause clause.next !
    clause ;

: clause.insert_literal ( literal clause1 -- clause2 ) recursive
    \ Inserts a literal in a clause if it does not occur.
    \
    \ "literal" The literal to be inserted.
    \ "clause1" The clause to which the literal is inserted.
    \ The address of the resulting clause might not be the address of
    \ the initial clause. The resulting clause "clause2" is ordered.
    { literal clause }
    clause 0= GUARD
        literal 0 clause.new_node END
    clause clause.literal @ literal literal.> GUARD
        literal clause clause.new_node END
    clause clause.literal @ literal literal.< GUARD
        literal clause clause.next @ clause.insert_literal
        clause clause.next !
        clause END        
    clause ;

: clause.show_literal' ( clause -- )
    \ Shows the literal associated to a clause node.
    \
    \ "clause" The node whose literal is showed.
    clause.literal @ 1 .r ;

: clause.show ( clause -- )
    \ Prints a clause.
    \
    \ "clause" The clause that is printed.
    { clause } [Char] ] clause bl ['] clause.show_literal' [Char] [ list.show ;

: clause.literal.= ( clause1 clause2 -- f )
    \ Compares two clauses by their head literal.
    \
    \ Compares the literals stored in the first node of two non-empty
    \ clauses. If the literals are equal then f is true, otherwise f
    \ is false.
    clause->literal swap clause->literal swap literal.= ;

: clause.literal.<
    \ Compares two clauses by their head literal.
    \
    \ Compares the literals stored in the first node of two non-empty
    \ clauses. If the first literal is strictly less than the second
    \ one, then f is true, otherwise f is false.
    clause->literal swap clause->literal swap literal.< ;

: clause.merge.select' { clause1 clause2 -- c1' c2' l }
    \ Selects the values for recursion of the clause merge algorithm.
    clause1 clause2 clause.literal.= GUARD
         clause1 clause.next @ clause2 clause.next @ clause1 clause.literal @ END
    clause1 clause2 clause.literal.< GUARD
         clause1 clause.next @ clause2 clause1 clause->literal END
    clause1 clause2 clause.next @ clause2 clause.literal @ ;

: clause.merge' recursive { result clause1 clause2 -- }
    \ Merges two clauses in linear time.
    \
    \ Merges the two given clauses and appends the result to 'result.
    clause1 list.is_empty? GUARD
        clause2 clause.copy result clause.next ! END
    clause2 list.is_empty? GUARD
        clause1 clause.copy result clause.next ! END
    clause1 clause2 clause.merge.select'
    0 clause.new_node dup result clause.next ! -rot
    clause.merge' ;

: clause.merge ( clause1 clause2 -- clause )
    \ Merges two clauses.
    \
    \ "clause1", "clause2" The clauses that are merged.
    \ The resulting clause is ordered and does not contain duplicate
    \ literals.
    0 0 clause.new_node dup 2swap clause.merge'
	clause.next @ ; \ todo take care of initial dummy element

: clause.equal? ( clause1 clause2 -- f ) recursive
    \ Compares two clauses for equality.
    \
    \ "clause1", "clause2" Adresses of the clauses that are compared.
    \ Returns true if both clauses contain the same literals, false
    \ otherwise.
    { clause1 clause2 }
    clause1 list.is_empty? GUARD
        clause2 0= IF true ELSE false ENDIF END
    clause2 list.is_empty? GUARD
        false END
    clause1 clause.literal @ clause2 clause.literal @ =
    clause1 clause.next @ clause2 clause.next @
    clause.equal? and ;

: clause.contains_literal? ( literal clause -- f)
    \ Checks whether a given clause contains a literal.
    \
    \ "literal" The literal to search for in the clause.
    \ "clause" The clause to be searched.
    \ Returns true if the clause contains the literal, false otherwise
    ['] clause.literal ['] = 2swap list.search ;

: clause.resolve ( literal clause1 clause2 -- resolvent )
    \ Resolves two clauses upon a literal.
    \
    \ "literal": The resolved upon literal; this is a literal of
    \ clause1 and its dual must appear in clause2.
    { literal clause1 clause2 }
    literal clause1 clause.copy clause.remove_literal { c1 } c1
    literal negate clause2 clause.copy clause.remove_literal { c2 } c2
    clause.merge c1 c2 clause.free clause.free ;

: clause.contains_dual? ( literal clause -- f )
    \ Checks whether the clause contains the dual of a given literal.
    \
    \ If the clause contains the dual then f is true, otherwise f is
    \ false.
    swap literal.dual swap clause.contains_literal? ;

: clause.resolve_full ( clause1 clause2 -- res_1 ... res_n n )
    \ Resolves two clauses upon all resolvable literals.
    \
    \ "clause1", "clause2" The clauses to be resolved. Leaves all the
    \ resolvents on the stack and the number of resolvents.
    { clause1 clause2 }
    0 clause1 BEGIN
        dup WHILE ( current )
            dup clause->literal clause2 clause.contains_dual? IF
                dup clause.literal @ clause1 clause2 clause.resolve
                rot 1+ rot
            ENDIF
            clause.next @
    REPEAT drop ;

: clause.subsumes_clause? ( c_2 c_1 -- f )
    \ Checks if a given clauses subsumes another clause.
    \
    \ Returns true if c_2 subsumes c_1, false otherwise.
    { c_1 } BEGIN dup 0<> WHILE
            dup clause.literal @ c_1 clause.contains_literal? invert IF
                drop false EXIT ENDIF
            list.next @
    REPEAT drop true ;

: clause.subsumed_by? ( c_1 c_2 -- f )
    \ Checks if a given clause is subsumed by another clause.
    \
    \ Returns true if c_1 is subsumed by c_2, false otherwise.
    swap clause.subsumes_clause? ;

: clause.compare_by_length ( clause1 clause2 -- f )
    \ Compares two clauses by length.
    \
    \ Returns true if the length of the first clause is strictly less
    \ than the length of the second clause.
    list.length swap list.length swap < ;
