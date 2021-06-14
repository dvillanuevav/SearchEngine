# Elasticsearch Autocomplete

Implementation of autocomplete functionality over [Elastissearch](https://www.elastic.co/elasticsearch/) using [Edge Ngrams](https://www.elastic.co/guide/en/elasticsearch/reference/current/analysis-edgengram-tokenizer.html).

## Requirements

* ASP.NET Core 3.1
* [MeaditR](https://github.com/jbogard/MediatR)
* [NEST](https://github.com/elastic/elasticsearch-net)
* [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## Overview

### Architecture

Having in mind some principles design like **Separation of Concerns**, **Single Responsibility**, and **Dependency Inversion** from **SOLID** this solution follows a [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) which comprises the layers below.

#### Domain

This layer contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

#### Application

This layer contains all application logic. It is dependent on the domain layer, but has no dependencies on any other layer. This layer defines interfaces that are implemented by outside layers.

#### Infrastructure

This layer contains classes for accessing external resources such as file systems, web services, smtp, and so on. These classes should be based on interfaces defined within the application layer.

#### Api

This layer depends on the application layer and allows to consume the application logic through an API REST.

### Elasticsearch Index

The Edge N-gram tokenizer has been set up for considering letter and digit characters, also two filters, one for excluding english stop words (e.g. the, and, or, into) and the other one for allowing lower case.

```
var createIndexDescriptor = new CreateIndexDescriptor(IndexName)
   .Settings(s => s
       .Analysis(a => a
           .Analyzers(an => an
               .Custom("edge_ngram_analyzer", ca => ca
                   .Tokenizer("edge_ngram_tokenizer")
                   .Filters("english_stop", "lowercase")
               )                           
           )
           .Tokenizers(to => to
               .EdgeNGram("edge_ngram_tokenizer", ng => ng.MaxGram(15).MinGram(2).TokenChars(new[] { TokenChar.Letter, TokenChar.Digit }))
           )
           .TokenFilters(tk => tk
               .Stop("english_stop", sw => sw.IgnoreCase(true).StopWords("_english_"))
           )
       )
   )
   .Map<RealEstateEntity>(m => m
       .Properties(props => props
           .Text(t => t
               .Name(p => p.Name)
               .Analyzer("edge_ngram_analyzer")
           )
           .Text(t => t
               .Name(p => p.FormerName)
               .Analyzer("edge_ngram_analyzer")
           )
           .Text(t => t
               .Name(p => p.State)
               .Analyzer("edge_ngram_analyzer")                           
           )
           .Text(t => t
               .Name(p => p.City)
               .Analyzer("edge_ngram_analyzer")
           )
           .Text(t => t
               .Name(p => p.StreetAddress)
               .Analyzer("edge_ngram_analyzer")
           )
           .Keyword(t => t
               .Name(p => p.Market)                           
           )
       )
   );
```

## Getting Started

Set up the the url for Elasticsearch at the `settings.json` and  run the project `SearchEngine.Autocomplete.Api` for displaying Swagger page.

### Indexing

First, hit the endpoint `/api/v1/real-estate-entities/indices/{maxItems}` for indexing the data into Elasticseach from the json files within the folder `\Data`.

> **maxItems**: Max number of items taking from each file.

### Searching

Second, hit the endpoint `/api/v1/real-estate-entities` passing the parameters describe below for searching management and properties entities  suggestions.

> **Keyword (Required)**: Term for searching coincidences along text fields.
>
> **Markets**: Filter one or more markets otherwise search all the markest.
>
> **PageIndex**: Page number. Default 1
>
> **PageSize**: Results per page. Default 25

## Other Alternatives

* [Prefix Query](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-prefix-query.html). 
* [Completion Suggester](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-suggesters.html#completion-suggester).
