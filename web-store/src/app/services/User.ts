export class User {
 id?: number;
  name?: string;
  email?: string;
  // add other relevant fields as needed

  constructor(init?: Partial<User>) {
    Object.assign(this, init);
  }
}