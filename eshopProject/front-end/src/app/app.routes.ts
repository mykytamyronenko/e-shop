import { Routes } from '@angular/router';
import {HomeComponent} from './core/layout/home/home.component';
import {LoginComponent} from './core/layout/login/login.component';
import {AdminComponent} from './core/admin/admin.component';
import {ProfilePageComponent} from './core/layout/profile-page/profile-page.component';
import {SellPageComponent} from './core/layout/sell-page/sell-page.component';
import {RegisterComponent} from './core/layout/register/register.component';
import {TradeComponent} from './features/articles/trade/trade.component';
import {MembershipPageComponent} from './core/layout/membership-page/membership-page.component'
import {FindUserComponent} from './core/layout/find-user/find-user.component';
import {
  EditOwnArticleFormComponent
} from './features/articles/articles-manager/components/edit-own-article-form/edit-own-article-form.component';


export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'hub', component: HomeComponent},
  { path: 'login', component: LoginComponent },
  { path: 'admin', component: AdminComponent },
  { path: 'profile-page/:username', component: ProfilePageComponent },
  { path: 'sell-page', component: SellPageComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'trade/:articleId', component: TradeComponent },
  { path: 'membership-page', component: MembershipPageComponent },
  { path: 'find-user', component: FindUserComponent },
  { path: 'editArticle/:articleId', component: EditOwnArticleFormComponent },
];
