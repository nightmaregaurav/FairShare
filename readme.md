# FairShare (Cost Splitter)
#### Gaurav's fair share problem & Reduction by Transitive Debt Algorithm
<hr/>

### Split shared expenses so that there is the least number of transactions.
### Maximum Transactions = `(n-1)` `WHERE` `n` = `number of members in group`.

## Context
* A group with several members is spending a certain amount of money.
* At some point in time they need to split the spending and make it so that everyone has a fair share.

## Do we really need a tool for that?
* <b>Let's assume different scenarios:</b>
  * <b>Group of 2 (`A`, `B`)</b> `WHERE` `A` is doing all the spending.
    * Just divide the expense by 2 and that's how much `B` owes to `A` <i><b>(Max: One transaction)</b></i>
  * <b>Group of 2 (`A`, `B`)</b> `WHERE` `A` & `B` both are spending.
    * Just calculate the total expense from each
    * Divide the expense of each by 2 and that's how much they owe to each other. <i><b>(Max 2 transactions)</b></i>
    * Subtract from who owes the least from the most. <i><b>(Max: One transaction)</b></i>
  * <b>Group of 3 (`A`, `B`, `C`)</b> `WHERE` any of them can be spending.
    * Calculate the total expense from each
    * Divide the expense of each by 3 and that's how much they owe to each other. <i><b>(Max 6 transactions)</b></i>
    * Subtract each other's payment and receipt. <i><b>(Max: Three transaction)</b></i>
  * <b>Group of 4 or more?</b>
    * No I don't need to tell you but this is going to be nasty quickly as complexity rises with number.

#### But what if I told you in a group of three, the calculation can be done in such a way that there is only 2 transaction at most?
#### This tool (and algorithm) will allow you to split these costs so that there are at most `n-1` transactions where `n` is the number of members in the group. 

## Problem Statement (Let's call it `Gaurav's fair share problem`)
  - Given a group of `n` people. Spending independently for the group while keeping a record of all expenses. Decide to split all the expenses and settle the payments they have. Settle the payment in such a way that there are no more than `n-1` transactions at most.

## Proof of Concept
### Let's assume we have a group of 5 (`A`, `B`, `C`, `D`, `E`) with given expenses:
| Head                   | Cost  | By  |
|------------------------|-------|-----|
| Some stuff             | $560  | `A` |
| Some other stuff       | $278  | `E` |
| Some good stuff        | $1078 | `B` |
| Some bad stuff         | $28   | `C` |
| Some useful stuff      | $238  | `C` |
| Some really good stuff | $2278 | `D` |

### So in total:
| Head  | Cost  | By  |
|-------|-------|-----|
| Total | $560  | `A` |
| Total | $1078 | `B` |
| Total | $266  | `C` |
| Total | $2278 | `D` |
| Total | $278  | `E` |

### Now, we can split those totals for each member and write them in the form of a matrix, the column being the payer and, the row being the receiver.
<b>(We can omit the diagonal values as that is to be paid by oneself)</b>

| R\S | `A`    |  `B`   |  `C`   |  `D`   |  `E`   |
|-----|--------|--------|--------|--------|--------|
| `A` | 0.00   | 112.00 | 112.00 | 112.00 | 112.00 |
| `B` | 215.60 | 0.00   | 215.60 | 215.60 | 215.60 |
| `C` | 53.20  | 53.20  | 0.00   | 53.20  | 53.20  |
| `D` | 455.60 | 455.60 | 455.60 | 0.00   | 455.60 |
| `E` | 55.60  | 55.60  | 55.60  | 55.60  | 0.00   |

### This basically means
<b>(Format: `Payer` &rarrlp; `Receiver` [`Amount`])</b>
  * `A` &rarrlp; `B` [`215.6`]
  * `A` &rarrlp; `C` [`53.2`]
  * `A` &rarrlp; `D` [`455.6`]
  * `A` &rarrlp; `E` [`55.6`]
  * `B` &rarrlp; `A` [`112`]
  * `B` &rarrlp; `C` [`53.2`]
  * `B` &rarrlp; `D` [`455.6`]
  * `B` &rarrlp; `E` [`55.6`]
  * `C` &rarrlp; `A` [`112`]
  * `C` &rarrlp; `B` [`215.6`]
  * `C` &rarrlp; `D` [`455.6`]
  * `C` &rarrlp; `E` [`55.6`]
  * `D` &rarrlp; `A` [`112`]
  * `D` &rarrlp; `B` [`215.6`]
  * `D` &rarrlp; `C` [`53.2`]
  * `D` &rarrlp; `E` [`55.6`]
  * `E` &rarrlp; `A` [`112`]
  * `E` &rarrlp; `B` [`215.6`]
  * `E` &rarrlp; `C` [`53.2`]
  * `E` &rarrlp; `D` [`455.6`]

### Now, as we can see there are redundant transactions
  * It is obvious that if `A` has to pay  `B` `$10` and `B` has to pay `A` `$4` rather than doing 2 transactions, we can just do one, that is `A` pays `B` `$6`.
  * Let's do the same here.
  * For position `(i, j)`
    * if `i != j` and `(j ,i) != 0` and `(i, j) > (j, i)`
      * `(i, j) = (i, j) - (j, i)`
      * `(j, i) = 0`

### Now we have a reduced matrix
| R\S | `A`    | `B`    | `C`    | `D`   | `E`    |
|-----|--------|--------|--------|-------|--------|
| `A` | 0.00   | 0.00   | 58.80  | 0.00  | 56.40  |
| `B` | 103.60 | 0.00   | 162.40 | 0.00  | 160.00 |
| `C` | 0.00   | 0.00   | 0.00   | 0.00  | 0.00   |
| `D` | 343.60 | 240.00 | 402.40 | 0.00  | 400.00 |
| `E` | 0.00   | 0.00   | 2.40   | 0.00  | 0.00   |

### This means
<b>(Format: `Payer` &rarrlp; `Receiver` [`Amount`])</b>
  * `A` &rarrlp; `B` [`103.6`]
  * `A` &rarrlp; `D` [`343.6`]
  * `B` &rarrlp; `D` [`240.00000000000003`]
  * `C` &rarrlp; `A` [`58.8`]
  * `C` &rarrlp; `B` [`162.39999999999998`]
  * `C` &rarrlp; `D` [`402.40000000000003`]
  * `C` &rarrlp; `E` [`2.3999999999999986`]
  * `E` &rarrlp; `A` [`56.4`]
  * `E` &rarrlp; `B` [`160`]
  * `E` &rarrlp; `D` [`400`]

### WooHoo! Reduced a lot, but still not `n-1` as I promised right?
### If you have a close look, there is nowhere to reduce as we did before. But That's where the magic kicks in!

### The matrix shows how much one has to pay in the column and receive in the row, let's transpose to change it.
| S\R | A     | B      | C    | D      | E    |
|-----|-------|--------|------|--------|------|
| `A` | 0.00  | 103.60 | 0.00 | 343.60 | 0.00 |	
| `B` | 0.00  | 0.00   | 0.00 | 240.00 | 0.00 |	
| `C` | 58.80 | 162.40 | 0.00 | 402.40 | 2.40 |
| `D` | 0.00  | 0.00   | 0.00 | 0.00   | 0.00 |
| `E` | 56.40 | 160.00 | 0.00 | 400.00 | 0.00 |

### Since we don't have the option to reduce directly, let's look at a scenario:
  * `X` has to pay `Y` and `Z` `$10` and `$26` respectively, and `Y` has to pay 'Z' `$ABC`,
    * In a broad sense, this scenario is similar to what we have, there are no direct reductions.
    * But, what if, instead of paying both `Y` and `Z` their respective amount, `X` says:
      * Hey `Y`, since you have to pay something to `Z` anyway,
        * I will not pay `Z`
        * Instead pay you `$10 + $26 = $36`
        * You keep what you owe from me. ie. `$10`
        * Then you pay `Z` `$26` on my behalf and `$ABC` from your own. ie `$10 + $ABC`

### Looks like we found a new reduction method. (Let's call it `Reduction by Transitive Debt`)
### But, there are not only 3 members, so how can we transfer the debt?
  * Think of a queue where people are positioned based on number of people they have to pay to.
  * The person who has to pay the most people (Not necessarily the amount) is placed first, and so on.
  * So, the person on first can transit their debt to the person on second by sending the total amount they have to pay.
  * The person on the second will keep what the first person has to pay them, Add what they have to pay the person on back.
  * And the second person can send that sum to the third and so on.

### So, in our case
  * The order is `C`, `E`, `A`, `B`, `D`
    * The amount `C`, have to pay `E` is the sum of all debt 'C' has.
    * `E` Keeps what `C` owes `E`, Add what 'E' owes to `A`. `B`, and `D` and pay that Sum to `A`.
    * This continues till the end and all the debt is settled.

### Finally
| S\R | A       | B       | C    | D       | E      |
|-----|---------|---------|------|---------|--------|
| `A` | 0.00    | 1572.00 | 0.00 | 0.00    | 0.00   |		
| `B` | 0.00    | 0.00    | 0.00 | 1386.00 | 0.00   |		
| `C` | 0.00    | 0.00    | 0.00 | 0.00    | 626.00 |	
| `D` | 0.00    | 0.00    | 0.00 | 0.00    | 0.00   |	
| `E` | 1240.00 | 0.00    | 0.00 | 0.00    | 0.00   |	

### This means
<b>(Format: `Payer` &rarrlp; `Receiver` [`Amount`])</b>
  * `A` &rarrlp; `B` [`1572`]
  * `B` &rarrlp; `D` [`1386`]
  * `C` &rarrlp; `E` [`626`]
  * `E` &rarrlp; `A` [`1240`]

### This repo contains the C# implementation of this algorithm. Although the implementation may not be the most efficient one.

## License & Copyright
The code along with the problem statement and algorithm is released under the MIT License. You can find the full license details in the [LICENSE](LICENSE) file.

Made with ❤️ by [NightmareGaurav](https://github.com/nightmaregaurav).

---
Open For Contribution
---
