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

The `edge_ngram` tokenizer has been configured to treat letters and digits as tokens, and to produce grams with minimum length `2` and maximum length `30`. Also two analizers, one using the `edge_ngram` tokenizer at indexing time, to ensure that partial words are available for matching in the index and the another one at search time, just search for the terms the user has typed in.

```csharp
var createIndexDescriptor = new CreateIndexDescriptor(IndexName)
   .Settings(s => s
       .Analysis(a => a
           .Analyzers(an => an
               .Custom("autocomplete", ca => ca
                   .Tokenizer("edge_ngram_tokenizer")
                   .Filters("lowercase")
               )
                .Custom("autocomplete_search", ca => ca                               
                   .Tokenizer("lowercase")
                   .Filters("english_stop")
               )
           )
           .Tokenizers(to => to
               .EdgeNGram("edge_ngram_tokenizer", ng => 
               ng.MaxGram(30)
               .MinGram(2)
               .TokenChars(new[] { TokenChar.Letter, TokenChar.Digit }))
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
               .Analyzer("autocomplete")
               .SearchAnalyzer("autocomplete_search")
           )
           .Text(t => t
               .Name(p => p.FormerName)
               .Analyzer("autocomplete")
               .SearchAnalyzer("autocomplete_search")
           )
           .Text(t => t
               .Name(p => p.State)
               .Analyzer("autocomplete")
               .SearchAnalyzer("autocomplete_search")
           )
           .Text(t => t
               .Name(p => p.City)
               .Analyzer("autocomplete")
               .SearchAnalyzer("autocomplete_search")
           )
           .Text(t => t
               .Name(p => p.StreetAddress)
               .Analyzer("autocomplete")
               .SearchAnalyzer("autocomplete_search")
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

> **maxItems**: Max number of items taking from each file. Default 20000

### Searching

Second, hit the endpoint `/api/v1/real-estate-entities` passing the parameters describe below for searching management and properties entities  suggestions.

> **Keyword (Required)**: Term for searching coincidences along text fields.
>
> **Markets**: Filter one or more markets otherwise search all the markest.
>
> **PageIndex**: Page number. Default 1
>
> **PageSize**: Results per page. Default 25

#### Example

Searching coincidences for keyword `Apartmemt and hom` (with a typo, stopword and lowercase) and markets `San Antonio,  San Francisco, Austin and Los Angeles`.

**Curl**

```
curl -X 'GET' \
  'https://localhost:44367/api/v1/real-estate-entities?Keyword=Aparamemt%20and%20home&Markets=San%20Antonio&Markets=San%20Francisco&Markets=Austin&Markets=Los%20Angeles&PageIndex=1&PageSize=5' \
  -H 'accept: text/plain'    
```

**Response**

```json
{
  "items": [
    {
      "id": -1714990673,
      "code": 77390,
      "name": "Marquis at TPC",
      "formerName": "CWS Apartment Homes",
      "market": "San Antonio",
      "state": "TX",
      "city": "San Antonio",
      "streetAddress": "5505 TPC Pkwy",
      "type": 2
    },
    {
      "id": 201192595,
      "code": 70547,
      "name": "Legacy Apartment Homes",
      "formerName": "Legacy Apartment Homes",
      "market": "San Antonio",
      "state": "TX",
      "city": "San Antonio",
      "streetAddress": "11300 Roszell",
      "type": 2
    },
    {
      "id": 1090513977,
      "code": 115383,
      "name": "Tower Apartment Homes",
      "formerName": "",
      "market": "San Francisco",
      "state": "CA",
      "city": "Alameda",
      "streetAddress": "2465 Shoreline Dr",
      "type": 2
    },
    {
      "id": -359598365,
      "code": 115960,
      "name": "Portofino Apartment Homes",
      "formerName": "",
      "market": "San Francisco",
      "state": "CA",
      "city": "Pittsburg",
      "streetAddress": "500 Loveridge Circle",
      "type": 2
    },
    {
      "id": 329272150,
      "code": 120690,
      "name": "Lincoln Villa | Apartment Homes Marin",
      "formerName": "Lincoln Villa",
      "market": "San Francisco",
      "state": "CA",
      "city": "San Rafael",
      "streetAddress": "1825 Lincoln Avenue",
      "type": 2
    }
  ],
  "totalPages": 4,
  "totalCount": 20,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "pageIndex": 1,
  "pageSize": 5
}
```


## Other Alternatives

* [Prefix Query](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-prefix-query.html). 
* [Completion Suggester](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-suggesters.html#completion-suggester).
