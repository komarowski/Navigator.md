# Welcome to Navigator.md!

## Basic Syntax

```markdown
- I just love **bold text**.
- Italicized text is the *cat's meow*.

1. This text is ***really important***.
2. Text with `code text`
3. Text with [link to wikipedia](https://www.wikipedia.org/) 
```

- I just love **bold text**.
- Italicized text is the *cat's meow*.

1. This text is ***really important***.
2. Text with `code text`
3. Text with [link to wikipedia](https://www.wikipedia.org/) 

## Details with code block

````markdown
@@details Sql query to get information about columns

The `INFORMATION_SCHEMA.COLUMNS` view allows you to get information about all columns.

```sql
SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME='TableName'
```

@@
````

@@details Sql query to get information about columns

The `INFORMATION_SCHEMA.COLUMNS` view allows you to get information about all columns.

```sql
SELECT *
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME='TableName'
```

@@

## Image slider

```markdown
@@slider
![image.jpg](You can use local images)
![image_link](Or you can use image from Internet)
@@
```

@@slider
![https://upload.wikimedia.org/wikipedia/commons/thumb/d/d2/C_Sharp_Logo_2023.svg/1200px-C_Sharp_Logo_2023.svg.png]()
![https://upload.wikimedia.org/wikipedia/commons/thumb/9/99/Unofficial_JavaScript_logo_2.svg/1200px-Unofficial_JavaScript_logo_2.svg.png]()
@@

## Blockquotes

```markdown
> Dorothy followed her through many of the beautiful rooms in her castle.
>
> The Witch bade her clean the pots and kettles and sweep the floor and keep the fire fed with wood.
```

> Dorothy followed her through many of the beautiful rooms in her castle.
>
> The Witch bade her clean the pots and kettles and sweep the floor and keep the fire fed with wood.