import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserExamService } from '../../services/user-exam.service';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.scss']
})
export class ReviewComponent implements OnInit {
  examId!: number;
  reviewData: any;
  currentDate: string = new Date().toLocaleDateString();

  constructor(private route: ActivatedRoute, private userExamService: UserExamService) {}

  ngOnInit(): void {
    this.examId = +this.route.snapshot.paramMap.get('id')!;
    this.userExamService.getReview(this.examId).subscribe((data) => {
      this.reviewData = data;
    });
  }

  downloadPDF(): void {
    const content = document.getElementById('review-container');
    if (!content) return;

    html2canvas(content).then((canvas) => {
      const imgData = canvas.toDataURL('image/png');
      const pdf = new jsPDF('p', 'mm', 'a4');
      const width = pdf.internal.pageSize.getWidth();
      const height = (canvas.height * width) / canvas.width;
      pdf.addImage(imgData, 'PNG', 0, 0, width, height);
      pdf.save(`Review_Report_${this.currentDate}.pdf`);
    });
  }
}
