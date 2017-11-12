\ most significant bit
: msb
	dup 0 <> if
        1 RSHIFT 1 begin
	    	over 0 <> while
			swap 1 RSHIFT
			swap 1 LSHIFT
		repeat
		swap drop
	endif ;
	
	
: resolve { a a_pos a_neg b b_pos b_neg }
	a_neg b_pos and a_pos b_neg and or a b and and msb
	invert
	a b or over and
	swap
	a_pos b_pos or over and
	swap
	a_neg b_neg or over and
	swap drop ;
	
	
variable c0
7 c0 !

variable c0_pos
7 c0_pos !

variable c0_neg
0 c0_neg !


variable c1
25 c1 !

variable c1_pos
24 c1_pos !

variable c1_neg
1 c1_neg !


variable c2
28 c2 !

variable c2_pos
24 c2_pos !

variable c2_neg
4 c2_neg !


: test  c0 @ c0_pos @ c0_neg @ c1 @ c1_pos @ c1_neg @ ;
: test2 c0 @ c0_pos @ c0_neg @ c2 @ c2_pos @ c2_neg @ ;

test2 resolve .s