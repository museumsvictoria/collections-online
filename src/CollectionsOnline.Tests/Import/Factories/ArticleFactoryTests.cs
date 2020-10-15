using System.Collections;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Tests.Fakes;
using NSubstitute;
using Raven.Tests.Helpers;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class ArticleFactoryTests : RavenTestBase
    {
        [Fact]
        public void GivenNewArticle_UpdateDocument_UpdatesExistingArticleRelatedItems()
        {
            // Given
            var existingArticle = FakeArticles.CreateFakeArticle("articles/1");            
            existingArticle.RelatedItemIds = new[] { "items/23", "items/168" };

            var item1 = FakeItems.CreateFakeItem("items/23");
            item1.RelatedArticleIds = new[] { "articles/1", "articles/4212" };

            var item2 = FakeItems.CreateFakeItem("items/168");
            item2.RelatedArticleIds = new[] { "articles/1", "articles/1212" };

            var item3 = FakeItems.CreateFakeItem("items/230");
            item3.RelatedArticleIds = new[] { "articles/1212" };

            var dataToBeSeeded = new List<IEnumerable>
                {
                    new[] { existingArticle },
                    new[] { item1, item2, item3 }
                };

            using (var documentStore = NewDocumentStore(seedData: dataToBeSeeded))
            {
                var articleFactory = new ArticleFactory(Substitute.For<IMediaFactory>(), Substitute.For<ISummaryFactory>());

                // When
                using (var documentSession = documentStore.OpenSession())
                {
                    existingArticle = documentSession.Load<Article>("articles/1");

                    var newArticle = FakeArticles.CreateFakeArticle("articles/1");
                    newArticle.RelatedItemIds = new[] { "items/168", "items/230" };

                    articleFactory.UpdateDocument(newArticle, existingArticle, new List<string>(), documentSession);
                    documentSession.SaveChanges();
                }

                // Then
                using (var documentSession = documentStore.OpenSession())
                {
                    var updatedItem1 = documentSession.Load<Item>("items/23");
                    updatedItem1.RelatedArticleIds.ShouldNotContain("articles/1");

                    var updatedItem3 = documentSession.Load<Item>("items/230");
                    updatedItem3.RelatedArticleIds.ShouldContain("articles/1");
                }
            }
        }

        [Fact]
        public void GivenNewArticle_UpdateDocument_UpdatesExistingArticleRelatedSpecimens()
        {
            // Given
            var existingArticle = FakeArticles.CreateFakeArticle("articles/1");
            existingArticle.RelatedSpecimenIds = new[] { "specimens/23", "specimens/168" };

            var specimen1 = FakeSpecimens.CreateFakeSpecimen("specimens/23");
            specimen1.RelatedArticleIds = new[] { "articles/1", "articles/4212" };

            var specimen2 = FakeSpecimens.CreateFakeSpecimen("specimens/168");
            specimen2.RelatedArticleIds = new[] { "articles/1", "articles/1212" };

            var specimen3 = FakeSpecimens.CreateFakeSpecimen("specimens/230");
            specimen3.RelatedArticleIds = new[] { "articles/1212" };

            var dataToBeSeeded = new List<IEnumerable>
                {
                    new[] { existingArticle },
                    new[] { specimen1, specimen2, specimen3 }
                };

            using (var documentStore = NewDocumentStore(seedData: dataToBeSeeded))
            {
                var articleFactory = new ArticleFactory(Substitute.For<IMediaFactory>(), Substitute.For<ISummaryFactory>());

                // When
                using (var documentSession = documentStore.OpenSession())
                {
                    existingArticle = documentSession.Load<Article>("articles/1");

                    var newArticle = FakeArticles.CreateFakeArticle("articles/1");
                    newArticle.RelatedSpecimenIds = new[] {"specimens/168", "specimens/230"};

                    articleFactory.UpdateDocument(newArticle, existingArticle, new List<string>(), documentSession);
                    documentSession.SaveChanges();
                }

                // Then
                using (var documentSession = documentStore.OpenSession())
                {
                    var updatedSpecimen1 = documentSession.Load<Specimen>("specimens/23");
                    updatedSpecimen1.RelatedArticleIds.ShouldNotContain("articles/1");

                    var updatedSpecimen3 = documentSession.Load<Specimen>("specimens/230");
                    updatedSpecimen3.RelatedArticleIds.ShouldContain("articles/1");
                }
            }
        }

        [Fact]
        public void GivenNewArticle_UpdateDocument_UpdatesExistingParentArticle()
        {
            // Given
            var parentArticle1 = FakeArticles.CreateFakeArticle("articles/1348");
            parentArticle1.ChildArticleIds = new[] { "articles/2126", "articles/2124" };

            var parentArticle2 = FakeArticles.CreateFakeArticle("articles/848");

            var childArticle1 = FakeArticles.CreateFakeArticle("articles/2126");
            childArticle1.ParentArticleId = "articles/1348";

            var childArticle2 = FakeArticles.CreateFakeArticle("articles/2124");
            childArticle2.ParentArticleId = "articles/1348";

            var dataToBeSeeded = new List<IEnumerable>
                {
                    new[] { parentArticle1, parentArticle2, childArticle1, childArticle2 }
                };

            using (var documentStore = NewDocumentStore(seedData: dataToBeSeeded))
            {
                var articleFactory = new ArticleFactory(Substitute.For<IMediaFactory>(),
                    Substitute.For<ISummaryFactory>());

                // When
                using (var documentSession = documentStore.OpenSession())
                {
                    childArticle1 = documentSession.Load<Article>("articles/2126");

                    var newChildArticle1 = FakeArticles.CreateFakeArticle("articles/2126");
                    newChildArticle1.ParentArticleId = "articles/848";

                    articleFactory.UpdateDocument(newChildArticle1, childArticle1, new List<string>(), documentSession);
                    documentSession.SaveChanges();
                }

                // Then
                using (var documentSession = documentStore.OpenSession())
                {
                    parentArticle1 = documentSession.Load<Article>("articles/1348");
                    parentArticle1.ChildArticleIds.ShouldNotContain("articles/2126");

                    parentArticle1 = documentSession.Load<Article>("articles/848");
                    parentArticle1.ChildArticleIds.ShouldContain("articles/2126");
                }
            }
        }
    }
}
