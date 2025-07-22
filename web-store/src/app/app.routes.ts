import { Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { LayoutComponent } from './components/layout/layout.component';
import { HomeComponent } from './components/home/home.component';
import { UserFormComponent } from './components/user-form/user-form.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { CardListComponent } from './components/card-list/card-list.component';
import { CardFormComponent } from './components/card-form/card-form.component';

export const routes: Routes = [
  { path: 'home', component: HomeComponent },
  { path: 'products', component: ProductListComponent, canActivate: [authGuard] },
  { path: 'add', component: ProductFormComponent, canActivate: [authGuard] },
  { path: 'users', component: UserListComponent, canActivate: [authGuard] },
  { path: 'adduser', component: UserFormComponent, canActivate: [authGuard] },
  { path: 'adduser/:id', component: UserFormComponent, canActivate: [authGuard] },


  { path: 'carts', component: CardListComponent, canActivate: [authGuard] },
  { path: 'addCart', component: CardFormComponent, canActivate: [authGuard] },
  { path: 'addCart/:id', component:CardFormComponent, canActivate: [authGuard] },


  { path: 'login', component: LoginComponent },
  { path: 'add/:id', component: ProductFormComponent, canActivate: [authGuard] },
  {
    path: 'layout', component: LayoutComponent,
    children: [
      {
        path: 'add', component: ProductFormComponent, canActivate: [authGuard]
      },
        { path: 'users', component: UserListComponent, canActivate: [authGuard] }
    ]
  },

  { path: '', redirectTo: '/products', pathMatch: 'full' },


  { path: '**', redirectTo: 'login', pathMatch: 'full' }

];
