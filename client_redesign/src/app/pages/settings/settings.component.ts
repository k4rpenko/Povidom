import { Component } from '@angular/core';
import {BorderMainComponent} from "../../components/border-main/border-main.component";
import {HEADERComponent} from "../../components/header/header.component";

@Component({
  selector: 'app-settings',
    imports: [
        BorderMainComponent,
        HEADERComponent
    ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {

}
