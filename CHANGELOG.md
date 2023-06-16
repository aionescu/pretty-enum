<!-- markdownlint-disable first-line-h1 -->

## v3.0.0 \[2023-06-16\]

* **\[Breaking\]** Dropped support for .NET Standard 2.0.
* **\[Breaking\]** Revamped parsing and pretty-printing logic.
  * Parsing is now case-sensitive, and `PreserveCaseAttribute` has been removed.
  * `PrettyNameAttribute`s and `DescriptionAttribute`s with a `null` or whitespace-only argument are now fully ignored (previously they had the same effect as `IgnorePrettyPrintAttribute`).
  * Pretty-printing undefined enum values is no longer supported.
* **\[Breaking\]** Removed non-generic APIs.
* Added nullability annotations to all exposed APIs.
* Improved performance, including caching of computed pretty names.
