export interface Article {
  articleId:number ;
  title:string ;
  description:string ;
  price:number ;
  category:string;
  state:'new' | 'used';
  userId:number ;
  createdAt:string;
  updatedAt:string ;
  status:'available' | 'sold' | 'removed';
  mainImageUrl:string;
  additionalImages:string;
  quantity:number ;
}
