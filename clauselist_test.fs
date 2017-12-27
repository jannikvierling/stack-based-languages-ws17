require list.fs
require clause.fs
require util.fs
require clauselist.fs

\ Tests the lists implemented in list.fs and provides some usage
\ examples.

\ List construction.

\ (1) Constructing a list in reverse order

0
list% %allot
list% %allot
list% %allot

dup constant head

list-next swap dup rot !
list-next swap dup rot !
list-next swap dup rot !

drop \ Drop the 0.

head list-length 3 = .

\ Test: list-search.

\ (1) Element not in list should not be found.
' clause-literal
' =
-1
1 2 3 4 5 6 0 6 times insert-literal
list-search invert .


\ (2) Element in list should be found.
' clause-literal
' =
5
1 2 3 4 5 6 0 6 times insert-literal
list-search .


\ Test: list-length.

\ (1) Empty list should have length zero.

0 list-length 0= .

\ (2) Nonempty list should have correct length.

list% %allot
list% %allot

swap over list-next !
dup 0 swap list-next @ list-next !

list-length 2 = .

\ Test: append-new

\ (1) New clause should be appended

1 2 3 0 3 times insert-literal
0 append-new

dup list-length 1 = .

clauselist-clause @
1 2 3 0 3 times insert-literal
clauses-equal .

\ (2) Redundant clause should not be appended

1 2 3 0 3 times insert-literal
1 2 3 0 3 times insert-literal
0 append-new append-new

dup list-length 1 = .

clauselist-clause @
1 2 3 0 3 times insert-literal
clauses-equal .

\ Test: contains-clause.

\ (1) Contained clause should be found.

-1 0 1 times insert-literal 
 1 0 1 times insert-literal 
0 2 times append-new
1 0 1 times insert-literal swap contains-clause .

\ (2) Non-contained clause should not be found.

-1 0 1 times insert-literal 
 1 0 1 times insert-literal 
0 2 times append-new
2 3 0 2 times insert-literal swap
contains-clause invert .

\ Test: insert-clause

1 0 1 times insert-literal
0
1 2 3 0 3 times insert-literal 
1 2 3 4 5 0 5 times insert-literal
4 0 insert-literal
1 2 0 2 times insert-literal
0
3 0 insert-literal
0 8 times insert-clause


dup list-length 7 = .

dup clauselist-clause @ list-length 0= .
list-next @

dup clauselist-clause @ list-length 1 = .
list-next @

dup clauselist-clause @ list-length 1 = .
list-next @

dup clauselist-clause @ list-length 1 = .
list-next @

dup clauselist-clause @ list-length 2 = .
list-next @

dup clauselist-clause @ list-length 3 = .
list-next @

dup clauselist-clause @ list-length 5 = .
list-next @ 0= .

\ Test: show-clauselist

-1 2 -3 -5 4 6 10 0 7 times insert-literal 
 1 0 1 times insert-literal 
0 2 times append-new
show-clauselist