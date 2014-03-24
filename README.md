LabRat
======

Give names to your experiments, and test for whether or not something is in that experiment.

### In the Database ###

Get raw number from input Id and Experiment Name. Mod this by the number of groups (probably 100) to determine in the database whether something was in the experiment.

``` SQL
DECLARE @input as bigint
DECLARE @experiment as varchar

cast(
    cast(reverse(cast(SUBSTRING(
        hashbytes('md5',
            cast(reverse(cast(@input as binary(8))) as binary(8))
            + cast(@experiment as varbinary)),1,4) as binary(4))
        ) as binary(4)) as bigint)
```
