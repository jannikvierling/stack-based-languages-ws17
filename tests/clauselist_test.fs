require ../util.fs
require ../list.fs
require ../clause.fs
require ../clauselist.fs

\ Tests the lists implemented in list.fs and provides some usage
\ examples.

\ List construction.

\ (1) Constructing a list in reverse order

0
list% %alloc
list% %alloc
list% %alloc

dup constant head

list.next swap dup rot !
list.next swap dup rot !
list.next swap dup rot !

drop \ Drop the 0.

head list.length 3 = .

\ Test: list.search.

\ (1) Element not in list should not be found.
' clause.literal
' =
-1
1 2 3 4 5 6 0 6 times clause.insert_literal
list.search invert .


\ (2) Element in list should be found.
' clause.literal
' =
5
1 2 3 4 5 6 0 6 times clause.insert_literal
list.search .


\ Test: list.length.

\ (1) Empty list should have length zero.

0 list.length 0= .

\ (2) Nonempty list should have correct length.

list% %alloc
list% %alloc

swap over list.next !
dup 0 swap list.next @ list.next !

list.length 2 = .

\ Test: clauselist.append_new

\ (1) New clause should be appended

1 2 3 0 3 times clause.insert_literal
0 clauselist.append_new

dup list.length 1 = .

clauselist.clause @
1 2 3 0 3 times clause.insert_literal
clause.equal? .

\ (2) Redundant clause should not be appended

1 2 3 0 3 times clause.insert_literal
1 2 3 0 3 times clause.insert_literal
0 clauselist.append_new clauselist.append_new

dup list.length 1 = .

clauselist.clause @
1 2 3 0 3 times clause.insert_literal
clause.equal? .

\ Test: clauselist.contains_clause?.

\ (1) Contained clause should be found.

-1 0 1 times clause.insert_literal 
 1 0 1 times clause.insert_literal 
0 2 times clauselist.append_new
1 0 1 times clause.insert_literal swap clauselist.contains_clause? .

\ (2) Non-contained clause should not be found.

-1 0 1 times clause.insert_literal 
 1 0 1 times clause.insert_literal 
0 2 times clauselist.append_new
2 3 0 2 times clause.insert_literal swap
clauselist.contains_clause? invert .

\ Test: clauselist.insert_clause

1 0 1 times clause.insert_literal
0
1 2 3 0 3 times clause.insert_literal 
1 2 3 4 5 0 5 times clause.insert_literal
4 0 clause.insert_literal
1 2 0 2 times clause.insert_literal
0
3 0 clause.insert_literal
0 8 times clauselist.insert_clause


dup list.length 7 = .

dup clauselist.clause @ list.length 0= .
list.next @

dup clauselist.clause @ list.length 1 = .
list.next @

dup clauselist.clause @ list.length 1 = .
list.next @

dup clauselist.clause @ list.length 1 = .
list.next @

dup clauselist.clause @ list.length 2 = .
list.next @

dup clauselist.clause @ list.length 3 = .
list.next @

dup clauselist.clause @ list.length 5 = .
list.next @ 0= .

\ Test: clauselist.show

-1 2 -3 -5 4 6 10 0 7 times clause.insert_literal 
 1 0 1 times clause.insert_literal 
0 2 times clauselist.append_new
clauselist.show