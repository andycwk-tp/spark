﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Spark.FileSystem;
using Spark.Tests.Stubs;

namespace Spark.Tests
{
    [TestFixture]
    public class PrefixSupportTester
    {
        private SparkSettings _settings;
        private SparkViewEngine _engine;

        [SetUp]
        public void Init()
        {
            _settings = new SparkSettings()
                .SetDebug(true)
                .SetPageBaseType(typeof(StubSparkView));

            _engine = new SparkViewEngine(_settings)
            {
                ViewFolder = new FileSystemViewFolder("Spark.Tests.Views")
            };
        }

        static void ContainsInOrder(string content, params string[] values)
        {
            int index = 0;
            foreach (string value in values)
            {
                int nextIndex = content.IndexOf(value, index);
                Assert.GreaterOrEqual(nextIndex, 0, string.Format("Looking for {0}", value));
                index = nextIndex + value.Length;
            }
        }

        [Test]
        public void PrefixFromSettings()
        {
            var settings = new SparkSettings()
                .SetDebug(true)
                .SetPageBaseType(typeof(StubSparkView))
                .SetPrefix("s");

            var engine = new SparkViewEngine(settings)
                             {
                                 ViewFolder = new FileSystemViewFolder("Spark.Tests.Views")
                             };

            var view = (StubSparkView)engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\prefix-from-settings.spark"));
            view.ViewData["Names"] = new[] { "alpha", "beta", "gamma" };

            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "<li", "alpha", "</li>",
                            "<li", "beta", "</li>",
                            "<li", "gamma", "</li>",
                            "<var x=\"1/0\">element ignored</var>",
                            "<p each=\"5\">attribute ignored</p>"
                );
        }


        [Test]
        public void PrefixFromXmlns()
        {
            var view = (StubSparkView)_engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\prefix-from-xmlns.spark"));
            view.ViewData["Names"] = new[] { "alpha", "beta", "gamma" };

            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "<li", "alpha", "</li>",
                            "<li", "beta", "</li>",
                            "<li", "gamma", "</li>",
                            "<var x=\"1/0\">element ignored</var>",
                            "<p each=\"5\">attribute ignored</p>");
        }


        [Test]
        public void ConditionalAttributes()
        {
            var view = (StubSparkView)_engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\conditional-attributes.spark"));
            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "ok1",
                            "ok2",
                            "ok3");

            Assert.IsFalse(output.ToString().Contains("fail"));
            Assert.IsFalse(output.ToString().Contains("if"));
            Assert.IsFalse(output.ToString().Contains("else"));
            Assert.IsFalse(output.ToString().Contains("condition"));
        }

        [Test]
        public void MacroAndContentPrefixes()
        {
            var view =
                (StubSparkView)
                _engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\macro-content-prefix.spark"));
            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "<p>one</p>",
                            "<p>two</p>",
                            "<p>Hello, world!</p>",
                            "<p>three</p>",
                            "<p>four</p>",
                            "<macro:ignored>ignored</macro:ignored>",
                            "<content:ignored>ignored</content:ignored>",
                            "<use:ignored>ignored</use:ignored>",
                            "<render:ignored>ignored</render:ignored>",
                            "<section:ignored>ignored</section:ignored>"
                );
        }

        [Test]
        public void SectionAndRenderPrefixes()
        {
            var view =
                (StubSparkView)
                _engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\section-render-prefix.spark"));
            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "<p>one</p>",
                            "<p>two</p>",
                            "<p>three</p>",
                            "<macro:ignored>ignored</macro:ignored>",
                            "<content:ignored>ignored</content:ignored>",
                            "<use:ignored>ignored</use:ignored>",
                            "<render:ignored>ignored</render:ignored>",
                            "<section:ignored>ignored</section:ignored>"
                );
        }

        [Test]
        public void MacroAndContentPrefixesFromSettings()
        {
            _engine.Settings = new SparkSettings()
                .SetDebug(true)
                .SetPageBaseType(typeof(StubSparkView))
                .SetPrefix("s");

            var view =
                (StubSparkView)
                _engine.CreateInstance(new SparkViewDescriptor().AddTemplate("Prefix\\macro-content-prefix-from-settings.spark"));
            var output = new StringWriter();
            view.RenderView(output);

            ContainsInOrder(output.ToString(),
                            "<p>one</p>",
                            "<p>two</p>",
                            "<p>Hello, world!</p>",
                            "<p>three</p>",
                            "<p>four</p>",
                            "<var x=\"1/0\">ignored</var>"
                );
        }
    }
}
