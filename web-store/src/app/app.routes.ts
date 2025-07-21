import { Routes } from '@angular/router';
import { ProductListComponent } from './components/product-list/product-list.component';
import { ProductFormComponent } from './components/product-form/product-form.component';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { LayoutComponent } from './components/layout/layout.component';

export const routes: Routes = [
  { path: 'products', component: ProductListComponent, canActivate: [authGuard] },
  { path: 'add', component: ProductFormComponent, canActivate: [authGuard] }, // add guard if needed
  { path: 'login', component: LoginComponent },
  { path: 'layout', component: LayoutComponent,
    children:[
      {
        path: 'add', component: ProductFormComponent, canActivate: [authGuard] 
      }
    ]
   },
  
  { path: '', redirectTo: '/products', pathMatch: 'full' },

  // Catch-all redirect
  { path: '**', redirectTo: 'login' , pathMatch: 'full'}

];
