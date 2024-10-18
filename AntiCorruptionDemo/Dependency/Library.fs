namespace Dependency

open System

module Internals =
    
    type Product (identifier: Guid, quantity: uint)  =
        member this.identifier = identifier
        member this.quantity = quantity
                
        interface IComparable with
            member this.CompareTo other =
                match other with
                | null -> 1
                | :? Product as other -> other.identifier |> this.identifier.CompareTo
                | _ -> invalidArg "other" "not a Product"
        
        member this.AddQuantity (quantity: uint) =
            Product(this.identifier, this.quantity + quantity)

        override this.Equals(other) =
            match other with
            | :? Product as other -> other.identifier = this.identifier && other.quantity = this.quantity
            | _ -> false
        
    
    type Order (products: Set<Product>) =
        member this.products = products
        
        member this.AddProduct(product: Product) =
             if this.products |> Set.exists (fun x -> x.identifier = product.identifier) then
                 this.AddQuantityTo(product.identifier, product.quantity)
             else
                 Order(this.products |> Set.add product)
                 
        member this.RemoveProduct(id: Guid) =
            Order(
                Set.filter (fun x -> not (x.identifier = id)) this.products
            )
        
        member this.AddQuantityTo(id: Guid, quantity: uint) =
            let newProduct =
                match this.GetProduct(id) with
                | Some product -> product.AddQuantity(quantity)
                | None -> Product(id, quantity)
            this.RemoveProduct(id).AddProduct(newProduct)            
            
        member this.Price =
            this.products |> Seq.sumBy(fun product -> 1.0 * float(product.quantity))
            // todo: get price of product from somewhere
            
            
        member this.GetProduct(id: Guid): Option<Product> =
            Seq.toList this.products |> List.tryFind (fun p -> p.identifier = id)
         