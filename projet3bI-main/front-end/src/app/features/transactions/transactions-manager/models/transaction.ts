export interface Transaction {
  transactionId:number ;
  buyerId:number ;
  sellerId:number ;
  articleId:number;
  transactionType:'purchase' | 'exchange';
  price:number ;
  commission:number ;
  transactionDate:string ;
  status:'in progress' | 'finished' | 'cancelled';
}
