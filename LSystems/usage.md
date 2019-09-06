# Quickstart on LSystems library usage
This document should provide you with information how to use LSystems library in your own project. All classes discussed
 here are part of `LSystems` Project and other Projects contained in this solution are not needed for correct function 
 of library.

*Note: To help differentiate, LSystems is used for project name and L-Systems is used for math concept*

## Basic Division
All classes are divided between two namespace, `LSystems.Core` and `LSystems.Util`. Former contains all classes which 
 are essential for generation of L-Systems and later contains classes which are related to L-Systems but are not related 
 to their generation. Contents of `LSystem.Core` are completely finished and should not change in future. Contents of
 `LSystems.Util` can be expanded in future or reworked.

### LSystems.Core
All classes in this namespace are immutable.
#### LSystems.Core.Module
All L-Systems must be formed above some alphabet. In this library alphabet is defined as collection of 
 `LSystem.Core.Module<T>` where each `Module` represents one character of alphabet. `Module<T>` is a generic class and 
 `T` corresponds to type used as value of arguments on each `Module`. Note that throughout all library there is no 
 support for using modules which differ in type of arguments. Each `Module` contains integer `Id` and array 
 `Parameters`. Id is used to identify letter of alphabet and should be unique for each letter (but it is on the user of 
 library to check which values he provides as `Id`). `Parameters` contains values of arguments.
 
Sometimes values for arguments are not known beforehand but some calculation must be done. To reflect this issue there
 is class `LSystems.Core.ModuleFactory<T>` which contains `Id` of new module and array of delegates where each delegate
 is used for one parameter of module. Typical usage of `ModuleFactory<T>` is in rules in parametric L-Systems where the
 right side of the rule often depends on the parameters defined on the left side.
 
#### LSystems.Core.Generation
`LSystems.Core.Generation<T>` is collection of modules which belong to one generation. It is basically proxy class for
 `List<Module<T>>` and is used when it is logical to talk about whole generation of L-Systems. 
 
`LSystems.Core.GenerationIndex<T>` is class containing `Generation` and index into this generation. It is used when
 when it is logical to talk about one module in context of whole generation (e.g. in context conditions).
 
#### LSystems.Core.Rule
Rules of L-System are represented by `LSystems.Core.Rule<T>` where T corresponds to type of arguments used in modules
 which are than used in rule. `Rule<T>` class represents all types of L-System rules (basic, context rules, parametric
 rules) and to decide which type of L-System rule is used depends only on arguments provided to `Rule<T>`.
 
`Rule<T>` consists of `int _sourceId` which determines letter in previous generation which will be overwritten and of 
 `ModuleFactory<T>[] _nextGenerationFactories` which describes modules which overwrite letter from previous generation.
 Optionally `ParamCondition[] _parametricConditions` can be provided if some parametric conditions are present. 
 `ParamCondition` is a delegate which returns `bool` and takes `T[]` which corresponds to values of parameters of source
 module. If more than one condition is provided, it is assumed that conditions are united by logical AND. Also 
 `ContextCondition[] _contextConditions` can be provided if some context conditions are present. `ContextCondition` is
 a delegate which returns `bool` and takes `GenerationIndex` which corresponds to index of source module in its 
 generation. If more than one condition is provided, it is assumed that conditions are united by logical AND. Also note
 that more than one context condition is possible which is not possible in L-Systems. This design decision is 
 deliberate.
 
`Rule` contains two methods. `bool CanBeApplied(GenerationIndex<T>)` which checks if rule can be applied by checking if
 `sourceId` matches id of module in generation index and checking all conditions. other method 
 `List<Module<T>> Apply(Module<T>)` will create new modules based on `_nextGenerationFactories` but does not check if 
 rule matches all conditions.

#### LSystems.Core.Generator
`LSystems.Core.Generator<T>` takes care of generation of L-Systems. It takes `Generation<T> axiom` which is used as
 0th generation and `Rule<T>[] rules` which represents all rules used in L-System generation. After calling method
 `AdvanceGeneration()` property `Generation<T> CurrentGeneration` is updated with newly generated modules.
 
### LSystems.Util
This section will not be described entirely as it parts can be changed. Only most important classes and ideas will be 
pointed out.

#### Parsers
Namespace `LSystems.Util.Parsers` contains classes useful for parsing string representation of L-Systems into 
 representations used in `LSystems.Core`. If whole `LSystems.Core.Generator<T>` is needed, `GeneratorParser` class can
 help. It assumes input is in form where axiom is on first line and than each line represents one rule until end of file
 is reached. Alphabet is not needed to define as the parser will deduce it from symbols used in representation. Rules 
 are expected in the form which is used on [Wikipedia article about L-Systems](https://en.wikipedia.org/wiki/L-system):
 ```
    Assumig A,B are letters of alphabet and x, y are values of parameters
    Basic:
    A -> AB
    Parametric:
    A(x, y) : x > 0 -> B(y+1)
    Context sensitive:
    A < B > A -> AB
 ```
 Note that every symbol of alphabet needs to start with mayor letter and than contain only lower letters, digits and 
 underscore.
 
Other classes which can be useful are located in file `LSystems/Core/Parsers/ExpressionParser.cs`. It is collection of
 parser which can transform mathematical expressions into delegates, which can be used as parametric conditions or in 
 `ModuleFactory`. This file contains predefined parsers for integer, real and bool values. Creating parsers for other 
 types should be easy, just be inheriting from abstract class `ExpressionParser<T>`. If you need any details how to 
 declare parsers for other types please look how are declared predefined parsers.
 
**Please note that if you known how your LSystems are gonna be generated during compile time, don't use these parsers. 
 These are meant to be used with user input and produce code which doesn't have to be optimal in terms of performance 
 for all inputs.**
 
#### Other
Currently only other file in namespace `LSystems.Util` is `VectorDrawer.cs` which contains drawer turning LSystems into 
 SVG image. More drawers or other utils may be added in future.