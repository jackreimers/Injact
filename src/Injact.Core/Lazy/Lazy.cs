namespace Injact;

//NOTES
//I want to use a virtual proxy for this so you don't need to litter codebases with Lazy<T>
//Likely going to need to use source generation to create a class dynamically that fits the signature of the lazily injected class without having to wrap it
//The benefit of this is that there should be no extra work required when creating a lazy binding, it can be marked and the container will handle creating the proxy

public class Lazy<T>
{
    
}