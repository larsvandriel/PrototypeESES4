import {Product} from './product.model';

export class Order {
  id: string;
  product: Product;
  status: string;

  constructor(
    id: string,
    product: Product,
    status: string
  ) {
    this.id = id;
    this.product = product;
    this.status = status;
  }
}
