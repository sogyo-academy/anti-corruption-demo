namespace Dependency

open System
open FSharpPlus

module Internals =
    
    type PriceList (prices: Map<Guid, float>) =
        member this.prices = prices
        member this.ForProduct(identifier: Guid) : Option<float> =
            this.prices.TryFind identifier
            
    
    type ProductQuantity (identifier: Guid, quantity: uint)  =
        member this.identifier = identifier
        member this.quantity = quantity
        member this.PriceGiven(prices: PriceList) : Option<float>=
            match prices.ForProduct(this.identifier) with
            | None -> None
            | Some price -> Some (price * float(this.quantity))  
    
    type Order (products: Map<Guid, ProductQuantity>) =
        member this.products = products
        
        member this.GetProduct(id: Guid): Option<ProductQuantity> =
            // Set.toList this.products |> List.tryFind (fun p -> p.identifier = id)
            this.products.TryFind id 
            
        member private this.RemoveProduct(id: Guid) : Order =
            Order(
                this.products.Remove id
            )
        
        member private this.AddProduct(product: ProductQuantity) : Order =
            Order(
                this.products.Add (product.identifier, product)
            )
            
        member private this.IncreaseQuantityOfPreExistingProduct(preExistingProduct: ProductQuantity, quantity: uint) : Order =
            let newProduct =
                ProductQuantity(
                    preExistingProduct.identifier, preExistingProduct.quantity + quantity
                )
            this.RemoveProduct(preExistingProduct.identifier).AddProduct(newProduct)
             
        member private this.LowerQuantityOfPreExistingProduct(preExistingProduct: ProductQuantity, quantity: uint) : Order =
            if preExistingProduct.quantity > quantity then
                let newProduct =
                    ProductQuantity(
                        preExistingProduct.identifier, preExistingProduct.quantity - quantity
                    )
                this.RemoveProduct(preExistingProduct.identifier).AddProduct(newProduct)
            else
                this.RemoveProduct(preExistingProduct.identifier)
            
        member this.AddToOrder(product: ProductQuantity) : Order =
             match this.GetProduct(product.identifier) with
             | Some preExistingProduct -> this.IncreaseQuantityOfPreExistingProduct(preExistingProduct, product.quantity)
             | None -> this.AddProduct(product)
             
        member this.RemoveFromOrder(product: ProductQuantity) : Order =
            match this.GetProduct(product.identifier) with
            | Some preExistingProduct -> this.LowerQuantityOfPreExistingProduct(preExistingProduct, product.quantity)
            | None -> this         
            
        member this.Price(prices: PriceList) : Result<float, string> =
            let henk(product: ProductQuantity) : Option<float> = product.PriceGiven(prices)
            let productPrices = traverse henk this.products.Values
            match productPrices with
            | Some productPrices -> Ok (Seq.sum productPrices)
            | _ -> Error "Price-list not exhaustive. Encountered product without price."
