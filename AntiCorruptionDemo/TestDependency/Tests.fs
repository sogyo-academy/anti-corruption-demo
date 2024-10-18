module Tests

open Dependency.Internals
open Xunit
open System

let TestOrderWithTwoProducts =
    Order(
        Set<Product>([
          Product(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"), uint 2);
          Product(Guid("99144c10-ea36-463d-bb5f-08102019a452"), uint 5)
        ])
    )
    
let TestOrderWIthDuplicateProducts =
    Order(
        Set<Product>([
          Product(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"), uint 2);
          Product(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"), uint 5)
        ])
    )


[<Fact>]
let ``By default, no discount is applied to an order`` () =
    let order = TestOrderWithTwoProducts
    let expected = 2.0 + 5.0
    Assert.Equal(expected, order.Price)

[<Fact>]
let ``An order with an extra product has a higher price`` () =
    let order = TestOrderWithTwoProducts
    let product: Product = Product(Guid(), uint 1)
    let biggerOrder = order.AddProduct(product)
    Assert.True(order.Price < biggerOrder.Price)
    
[<Fact>]
let ``Removing a product from the order decreases the price`` () =
    let order = TestOrderWithTwoProducts
    let smallerOrder = order.RemoveProduct(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"))
    Assert.True(order.Price > smallerOrder.Price)
 
[<Fact>]
let ``Adding a product to an order that already has that product increases its amount`` () =
    let id = Guid("99144c10-ea36-463d-bb5f-08102019a452")
    let order = TestOrderWithTwoProducts
    let product: Product = Product(id, uint 3)
    let biggerOrder = order.AddProduct(product)
    Assert.True(order.GetProduct(id).Value.quantity + uint 3 = biggerOrder.GetProduct(id).Value.quantity)
    Assert.Equal(uint 8, biggerOrder.GetProduct(id).Value.quantity)
    
[<Fact>]
let ``An order that has duplicate products is simplified to contain one product`` () =
    Assert.True(false) //todo: write this test!