using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CircularStack;
using System.Linq;
using KtExtensions;

namespace CircularStackUnitTest
{
    [TestClass]
    public class CircularStackTest
    {
        [TestMethod]
        public void Equals()
        {
            var d = new CircularStack<double>();
            for (int i = 0; i < 20; i++)
            {
                d.Add(i);
            }
            var e = new CircularStack<double>();
            for (int i = 0; i < 20; i++)
            {
                e.Add(i);
            }

            Assert.AreEqual(d, e);
        }

        [TestMethod]
        public void CheckNull()
        {
            TestClass c=null;
            Assert.IsTrue(c.IsNull());
        }

        [TestMethod]
        public void EqualityForClassType()
        {
            var stack1 = new CircularStack<TestClass>();
            for (int i = 0; i < 20; i++)
            {
                stack1.Add(new TestClass(i * 20.1));
            }
            var stack2 = new CircularStack<TestClass>();
            for (int i = 0; i < 20; i++)
            {
                stack2.Add(new TestClass(i * 20.1));
            }
            Assert.AreEqual(stack1, stack2);
        }

        [TestMethod]
        public void Constructor()
        {
            var stack = new CircularStack<int>();
            for (int i = 0; i < 4; i++)
            {
                stack.Add(i);
            }
            Assert.AreEqual(stack.Count,4);
        }

        [TestMethod]
        public void Append()
        {
            var stack = new CircularStack<int>();
            for (int i = 0; i < 4; i++)
            {
                stack.Add(i);
            }
            stack[0].Append(20);
            Assert.AreEqual(stack[1].Value, 20);
            stack[4].Append(20);
            Assert.AreEqual(stack[5].Value, 20);
            stack[3].Append(20);
            Assert.AreEqual(stack[4].Value, 20);

            Assert.AreEqual(stack[0].Value, 0);
            Assert.AreEqual(stack[1].Value, 20);
            Assert.AreEqual(stack[2].Value, 1);
            Assert.AreEqual(stack[3].Value, 2);
            Assert.AreEqual(stack[4].Value, 20);
            Assert.AreEqual(stack[5].Value, 3);
            Assert.AreEqual(stack[6].Value, 20);
        }

        [TestMethod]
        public void EnumeratorTest()
        {
            var stack = new CircularStack<int>(new[] { 1, 2, 3, 4, 5 });
            var i = 2;
            foreach (var sElent in stack[1].Take(4))
            {
                Assert.AreEqual(sElent, i);
                i++;
            }
            Assert.AreEqual(stack[1].Last(), 1);
        }

        [TestMethod]
        public void Preprend()
        {
            var stack = new CircularStack<int>();
            for (int i = 0; i < 4; i++)
            {
                stack.Add(i);
            }
            stack[1].Prepend(20);
            Assert.AreEqual(stack[1].Value, 20);
            stack.Add(20);
            Assert.AreEqual(stack[5].Value, 20);
            stack[4].Prepend(20);
            Assert.AreEqual(stack[4].Value, 20);

            Assert.AreEqual(stack[0].Value, 0);
            Assert.AreEqual(stack[1].Value, 20);
            Assert.AreEqual(stack[2].Value, 1);
            Assert.AreEqual(stack[3].Value, 2);
            Assert.AreEqual(stack[4].Value, 20);
            Assert.AreEqual(stack[5].Value, 3);
            Assert.AreEqual(stack[6].Value, 20);
        }
    }

    public class TestClass
    {
        private readonly double val;

        public TestClass(double val)
        {
            this.val = val;
        }

        public bool Equals(TestClass other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (this is null || other is null) return false;
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (this is null || obj is null) return false;
            if (GetType() != obj.GetType()) return false;
            return Equals((TestClass)obj);
        }

        public override int GetHashCode()
        {
            return 1835847388 + val.GetHashCode();
        }

        public static bool operator !=(TestClass a, TestClass b) => !(a == b);

        public static bool operator ==(TestClass a, TestClass b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return Math.Abs(a.val - b.val) < .000000000001;
        }
    }
}
