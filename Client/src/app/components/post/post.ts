import { Component, HostListener, inject, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Post } from '../../data/interface/Post/Post.interface';
import { CommonModule } from '@angular/common';
import { PostService } from '../../api/REST/post/Post.service';
import { PostCacheService } from '../../data/cache/post.service';

@Component({
  selector: 'app-post-component',
  imports: [CommonModule],
  templateUrl: './post.html',
  styleUrl: './post.scss',
})
export class PostComponent {
  @Input({ required: true }) loading!: boolean;
  @Input({ required: true }) posts!: Post[];
  Rest = inject(PostService);
  openedRepostMenuId: string | null = null;
  MessageText: string = "";
  MessageAction: boolean = false;

  constructor(
    private router: Router,
    private postCache: PostCacheService, 
    ) {}

  navigateToPost(id: string) {
    this.router.navigate(['/post', id]);
  }

  
  likePost(id: string, post: Post) {
    this.Rest.LikePost(id).subscribe({
      next: () => {
        post.likeAmount = (post?.likeAmount || 0) + 1;
        post.youLike = true;

        this.postCache.changPost(post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  DeleteLikePost(id: string, post: Post) {
    this.Rest.DeleteLikePost(id).subscribe({
      next: () => {
        post.likeAmount = (post?.likeAmount || 0) - 1;
        post.youLike = false;

        this.postCache.changPost(post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  
  likeComent(id: string, id_coment: string, post: Post) {
    this.Rest.LikeComent(id, id_coment).subscribe({
      next: () => {
        let coment = post.comments?.find(u => u.id === id_coment)!;
        coment.likeAmount++;
        coment.youLike = true;

        this.postCache.changPost(post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  DeletelikeComent(id: string, id_coment: string, post: Post) {
    this.Rest.DeleteLikeComent(id, id_coment).subscribe({
      next: () => {
        let coment = post.comments?.find(u => u.id === id_coment)!;
        coment.likeAmount--;
        coment.youLike = false;

        this.postCache.changPost(post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  
  savedPost(id: string) {
      const updatePost = this.posts.find(post => post.id === id)!;
      updatePost.youSaved = true;
  
      this.posts = this.posts.map(post => post.id === id ? updatePost : post);
      this.postCache.changPost(updatePost);
      this.Rest.SavedPost(id).subscribe({
        next: () => {},
        error: (error) => {
          const updatePost = this.posts.find(post => post.id === id)!;
          updatePost.youSaved = false;
  
          this.posts = this.posts.map(post => post.id === id ? updatePost : post);
          this.postCache.changPost(updatePost);
        }
      });
    }
  
    UnsavedPost(id: string) {
      const updatePost = this.posts.find(post => post.id === id)!;
      updatePost.youSaved = false;
  
      this.posts = this.posts.map(post => post.id === id ? updatePost : post);
      this.postCache.changPost(updatePost);
      this.Rest.DeleteSavedPost(id).subscribe({
        next: () => {},
        error: (error) => {
          const updatePost = this.posts.find(post => post.id === id)!;
          updatePost.youSaved = true;
  
          this.posts = this.posts.map(post => post.id === id ? updatePost : post);
          this.postCache.changPost(updatePost);
        }
      });
    }
  
    commentPost(id: string){
  
    }
  
    RepostMenu(event: MouseEvent, postId: string) {
      event.stopPropagation();
  
      this.openedRepostMenuId =this.openedRepostMenuId === postId ? null : postId;
    }
  
    AddRepost(post: Post){
      post.youRepost = true;
      post.repostAmount!++;
      this.Rest.Repost(post.id!).subscribe({
        next: () => {
  
        },
        error: () => {
          post.youRepost = false;
          post.repostAmount!--;
        }
      })
    }
  
    DeleteRepost(post: Post){
      post.youRepost = false;
      post.repostAmount!--;
      this.Rest.DeleteRepost(post.id!).subscribe({
        next: () => {
  
        },
        error: () => {
          post.youRepost = true;
          post.repostAmount!++;
        }
      })
    }
  
    CopyUrlPost(postId: string){
      const copy = `${window.location.origin}/post/${postId}`;
  
      navigator.clipboard.writeText(copy).then(async () => {
  
        this.MessageAction = true;
        this.MessageText = "link post copied";
  
        await delay(2000);
        this.MessageAction = false;
        await delay(1000);
  
        this.MessageText = "";
  
      }).catch(err => {
        console.error('Помилка при копіюванні:', err);
      });
    }

  @HostListener('document:click')
  closeMenus() {
    this.openedRepostMenuId = null;
  }

}

function delay(ms: number) {
    return new Promise( resolve => setTimeout(resolve, ms) );
}
