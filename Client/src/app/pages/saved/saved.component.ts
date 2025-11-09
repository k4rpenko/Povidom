import { Component } from '@angular/core';
import { HEADERComponent } from "../../components/header/header.component";
import { BorderMainComponent } from "../../components/border-main/border-main.component";

@Component({
  selector: 'app-saved',
  imports: [HEADERComponent, BorderMainComponent],
  templateUrl: './saved.component.html',
  styleUrl: './saved.component.scss'
})
export class SavedComponent {

  navigateToPost = () =>{
    console.log(1)
  }

  navigateToPostreply = () =>{
    console.log(2)
  }
}
