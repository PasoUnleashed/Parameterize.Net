using Parameterize.Net.Builder;
using Parameterize.Net.Reflection;

namespace Parameterize.Net.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    
    [Test]
    public void NumericResolverIntTest()
    {
        BasicNumericResolver resolver = BasicNumericResolver.Create<int>(1, 10);
        Assert.That(resolver.Resolve<int>(new float[]{1}), Is.EqualTo(1));
        Assert.That(resolver.Resolve<int>(new float[]{5}), Is.EqualTo(5));
        Assert.That(resolver.Resolve<int>(new float[]{10}), Is.EqualTo(10));
        Assert.That(resolver.Resolve<int>(new float[]{11}), Is.EqualTo(10));
        Assert.That(resolver.Resolve<int>(new float[]{5.5f}), Is.EqualTo(5));
        Assert.That(resolver.Resolve<int>(new float[]{5.8f}), Is.EqualTo(5));
    }

    [Test]
    public void NumericResolverFloatTest()
    {
        BasicNumericResolver resolver = BasicNumericResolver.Create<float>();
        Random r = new Random();
        for (int i = 0; i < 10000; i++)
        {
            float a = r.Next() / (float)int.MaxValue;
            Assert.That(resolver.Resolve<float>(new float[] { a }), Is.EqualTo(a));
        }
    }

    [Test]
    public void BoolResolverTest()
    {
        BasicNumericResolver resolver = BasicNumericResolver.Create<bool>();
        Assert.That(resolver.Resolve<bool>(new float[]{-1}), Is.EqualTo(false));
        Assert.That(resolver.Resolve<bool>(new float[]{0}), Is.EqualTo(true));
        Assert.That(resolver.Resolve<bool>(new float[]{1}), Is.EqualTo(true));
    }

    public class Tester
    {
        public float X;
    }
    [Test]
    public void SingleTypeResolverTest()
    {
        var resolver =ResolverBuilder.Model<Tester>().Property(i => i.X).Get();
        var result = resolver.Resolve<Tester>(new float[]{0.5f});
        Assert.That(result,Is.AssignableFrom(typeof(Tester)));
        Assert.That(result.X ,Is.EqualTo(0.5f));
    }

   

    [Test]
    public void RangeLerpTest()
    {
        Range r = new Range(1, 5);
        Assert.That(r.Lerp(0),Is.EqualTo(1));
        Assert.That(r.Lerp(1),Is.EqualTo(5));
    }

}