namespace md2visio.struc.classdiag
{
    internal enum ClsRelationType
    {
        Inheritance,    // <|--  filled triangle arrow + solid line
        Composition,    // *--   filled diamond + solid line
        Aggregation,    // o--   hollow diamond + solid line
        Association,    // -->   regular arrow + solid line
        Dependency,     // ..>   regular arrow + dashed line
        Realization,    // ..|>  hollow triangle arrow + dashed line
        Link,           // --    solid line without arrow
        DashedLink      // ..    dashed line without arrow
    }

    internal enum ClsVisibility
    {
        Public,         // +
        Private,        // -
        Protected,      // #
        Internal        // ~
    }
}
