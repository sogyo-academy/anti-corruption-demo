module Tests

open Dependency.Internals
open Xunit
open System

let TestOrderWithTwoProducts =
    Order(
        Map<Guid, ProductQuantity>(
            [ (Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"),
               ProductQuantity(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"), uint 2))
              (Guid("99144c10-ea36-463d-bb5f-08102019a452"),
               ProductQuantity(Guid("99144c10-ea36-463d-bb5f-08102019a452"), uint 5)) ]
        )
    )
    
let TestPriceList = 
        PriceList(Map<Guid, float>(
            [ (Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"),
               1.0)
              (Guid("99144c10-ea36-463d-bb5f-08102019a452"),
               1.0) ]
        ))


[<Fact>]
let ``By default, no discount is applied to an order`` () =
    let order = TestOrderWithTwoProducts

    let expected =
        Seq.sum [ for product in order.products.Values -> float product.quantity ]

    Assert.Equal(Ok expected, order.TotalPrice(TestPriceList))

[<Fact>]
let ``An order with an extra product has a higher price`` () =
    let order = TestOrderWithTwoProducts
    let product: ProductQuantity = ProductQuantity(Guid(), uint 1)
    let biggerOrder = order.AddToOrder(product)
    Assert.True(order.TotalPrice(TestPriceList) < biggerOrder.TotalPrice(TestPriceList))

[<Fact>]
let ``Removing a product from the order decreases the price`` () =
    let order = TestOrderWithTwoProducts

    let smallerOrder =
        order.RemoveFromOrder(ProductQuantity(Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe"), uint 2))

    Assert.True(order.TotalPrice(TestPriceList) > smallerOrder.TotalPrice(TestPriceList))

[<Fact>]
let ``Adding a product to an order that already has that product increases its amount`` () =
    let id = Guid("99144c10-ea36-463d-bb5f-08102019a452")
    let order = TestOrderWithTwoProducts
    let product: ProductQuantity = ProductQuantity(id, uint 3)
    let biggerOrder = order.AddToOrder(product)
    Assert.True(order.GetProduct(id).Value.quantity + uint 3 = biggerOrder.GetProduct(id).Value.quantity)
    Assert.Equal(uint 8, biggerOrder.GetProduct(id).Value.quantity)

[<Fact>]
let ``Removing a number of products from an order decreases the number of that product in the order`` () =
    let id = Guid("99144c10-ea36-463d-bb5f-08102019a452")
    let order = TestOrderWithTwoProducts
    let product: ProductQuantity = ProductQuantity(id, uint 2)
    let smallerOrder = order.RemoveFromOrder(product)
    Assert.True(smallerOrder.GetProduct(id).Value.quantity = order.GetProduct(id).Value.quantity - uint 2)

[<Fact>]
let ``Removing a number greater than the number of products in the order removes said product from the order`` () =
    let id = Guid("99144c10-ea36-463d-bb5f-08102019a452")
    let order = TestOrderWithTwoProducts
    let product: ProductQuantity = ProductQuantity(id, uint 9)
    let smallerOrder = order.RemoveFromOrder(product)
    Assert.True(smallerOrder.GetProduct(id).IsNone)

[<Fact>]
let ``Removing a product from an order does not touch other products`` () =
    let id = Guid("99144c10-ea36-463d-bb5f-08102019a452")
    let order = TestOrderWithTwoProducts
    let product: ProductQuantity = ProductQuantity(id, uint 9)
    let smallerOrder = order.RemoveFromOrder(product)
    Assert.True(Map.count smallerOrder.products = 1)
    let id = Guid("8704d51a-489d-4cb4-b29d-dd60eb1200fe")
    Assert.Equal(smallerOrder.GetProduct(id).Value.quantity, uint 2)
