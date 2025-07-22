import { Component, ViewChild } from '@angular/core';

import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { User } from '../../models/users';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule, MatSlideToggleModule, MatTableModule, MatCardModule,
    MatButtonModule, MatPaginatorModule, MatToolbarModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent {

  Users: User[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];
  dataSource = new MatTableDataSource<User>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  displayedColumns: string[] = ['id', 'email', 'username', 'name', 'phone', 'status', 'role', 'actions'];

  constructor(private userService: UserService, private router: Router) { }

  ngOnInit() {
    this.loadUsers();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }


  loadUsers() {
    this.userService.getUsers(this.currentPage, this.pageSize).subscribe(res => {
      this.Users = res.data;
      this.dataSource.data = this.Users;
      this.totalItems = res.totalItems;
    });
  }


  deleteUser(roduto: User) {
    if (roduto.id !== undefined) {
      this.userService.deleteUser(roduto.id).subscribe(() => {
        this.loadUsers();
      });
    } else {
      console.error('User ID is undefined. Cannot delete the User.');
    }
  }

  editUser(user: User) {

    this.router.navigate(['/adduser', user.id]);
  }

  onPageChange(event: any) {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

}