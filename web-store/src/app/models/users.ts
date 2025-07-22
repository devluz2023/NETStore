export interface Geolocation {
  lat: string;
  long: string;
}

export interface Address {
  city: string;
  street: string;
  number: number;
  zipcode: string;
  geolocation: Geolocation;
}

export interface Name {
  firstname: string;
  lastname: string;
}

export interface User {
  id?: number;
  email: string;
  username: string;
  password?: string;
  name: Name;
  address: Address;
  phone: string;
  status: 'Active' | 'Inactive' | 'Suspended';
  role: 'Customer' | 'Manager' | 'Admin';
}