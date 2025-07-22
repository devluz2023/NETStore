import { Product } from "./product";


export interface Cart {
  id?: number; // solo presente en respuestas o en edición
  date: string; // fecha en formato string (ej. ISO)
  products: Product[];
}