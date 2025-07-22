import { Product } from "./product";


export interface Cart {
  id?: number;
  date: string;
  products: Product[];
}