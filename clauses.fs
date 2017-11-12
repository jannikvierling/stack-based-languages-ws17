\ Allocates memory n+1 cells to store a clause of size n
\ Stores size of clause in first cell
\ Leaves address of first cell on stack
: make_clause { n -- address }
    here n here n 1+ cells allot ! ;


: check_range { address index -- }	
    index 0 < index address @ >= or throw ;

: read_variable { address index }
    address index check_range
	address cell+ index cells + @ ;
	
: write_variable { address index value -- }
    address index check_range
    value address cell+ index cells + ! ;
		
		
\ Takes a clause c0 of size n0 and a clause c1 of size n1
\ Returns a clause c of size n0 + n1 - 2 in which variable not
\ occure (neither positively or negatively)
\ Assumes that variable occures exactly once in both c0 and c1
\ Does not check if it occures positively in one clause and
\ negatively in the other
\ TODO: prune double occurrences of variables from result 
: resolve { address0 address1 variable -- address }
    address0 @ address1 @ + 2 - make_clause
    0 0 begin
        dup address0 swap read_variable dup abs variable abs <> if
            2over rot write_variable
            swap 1+ swap
        else
		    drop
        endif
	1+ dup address0 @ >= until
    drop 0 begin
        dup address1 swap read_variable dup abs variable abs <> if
            2over rot write_variable
            swap 1+ swap
        else
		    drop
        endif
	1+ dup address0 @ >= until 
	drop drop ;

\ Output a clause
: show_clause { address -- }
    0 begin
	    address over read_variable .
	1+ dup address @ >= until
    drop ;

\ Checks if a clause contains a variable (sign sensitive)
\ Unoptimized: always iterates through whole clause
: contains_variable { address variable -- flag }
    false 0 begin
	    address over read_variable variable = if
		    swap drop true swap
		endif
	1+ dup address @ >= until
	drop ;

: neg ( n -- -n ) -1 * ;
	
\ Finds a variable that is contained in both clauses
\ (once positively, once negatively)
\ If such a variable is found it is returned positively
\ If no such variable is found 0 is returned
\ Unoptimized: always iterates through whole clause
: can_resolve { address0 address1 -- variable }
    0 0 begin
	    address0 over read_variable dup neg
		address1 swap contains_variable if
		    abs rot drop swap
		else
		    drop
		endif
	1+ dup address0 @ >= until
	drop ;
	
create c0 3 ,  1 , 2 , 3 ,
create c1 3 , -1 , 4 , 5 ,

c0 c1 can_resolve

c0 c1 1 resolve show_clause
