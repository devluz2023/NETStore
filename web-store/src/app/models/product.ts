export interface Rating {
  rate: number;
  count: number;
}
export interface Product {
  id?: number; // optional for new products
  title: string;
  price: number;
  description: string;
  category: string;
  image: string;
  rating: Rating;
}