[CollectionDefinition("NoParallelization", DisableParallelization = true)]
public class NoParallelizationCollection() {}

[Collection("NoParallelization")]
public class MyTestClass() {}
