import { openDB, IDBPDatabase } from 'idb';

export class PostCache{
    private db: IDBPDatabase | undefined;
    constructor() {
        this.initDB();
    }

    private async initDB() {
        this.db = await openDB('posts-db', 1, {
            upgrade(db) {
                db.createObjectStore('posts', {
                    keyPath: 'id',
                    autoIncrement: true,
                });
            },
        });
    }


    public async addPost(post: Post, group: string): Promise<void> {
        if (!this.db) {
            throw new Error("Database is not initialized.");
        }
        post.hashtags = post.hashtags || [];
        if (!post.hashtags.includes(group)) {
            post.hashtags.push(group);
        }
        await this.db.add('posts', post);
    }

    public async getPostsByGroup(group: string): Promise<Post[]> {
        if (!this.db) {
            throw new Error("Database is not initialized.");
        }

        const posts = await this.db.getAll('posts');
        return posts.filter(post => post.hashtags && post.hashtags.includes(group));
    }


    public async deletePost(postId: string): Promise<void> {
        if (!this.db) {
            throw new Error("Database is not initialized.");
        }

        await this.db.delete('posts', postId);
        console.log(`Post with id ${postId} deleted.`);
    }
}