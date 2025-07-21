import { Component } from '@angular/core';

import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/users';


@Component({
  selector: 'app-user-list',
  imports: [CommonModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent {

Users: User[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];

  constructor(private userService: UserService,  private router: Router) {}

  ngOnInit() {
    this.loadUsers();

  }

  loadUsers() {
    this.userService.getUsers(this.currentPage, this.pageSize).subscribe(res => {
      this.Users = res.data;
      this.totalItems = res.totalItems;
    });
  }


  deleteUser(roduto:User) {
   if (roduto.id !== undefined) {
    this.userService.deleteUser(roduto.id).subscribe(() => {
      this.loadUsers();
    });
  } else {
    console.error('User ID is undefined. Cannot delete the User.');
  }
  }

    editUser(user:User) {
    // Aquí navegas a la vista del formulario de edición
    this.router.navigate(['/add', user.id]);
  }

  changePage(page: number) {
    this.currentPage = page;
    this.loadUsers();
  }
}